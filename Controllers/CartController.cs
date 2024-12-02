using Microsoft.AspNetCore.Mvc;
using ItalianCharmBracelet.Data;
using ItalianCharmBracelet.ViewModels;
using ItalianCharmBracelet.Helpers;
using Microsoft.AspNetCore.Authorization;
using System.Text.Json.Nodes;
using Azure;
using ItalianCharmBracelet.Services;

namespace ItalianCharmBracelet.Controllers
{
    public class CartController : Controller
    {
        private readonly PaypalClient _paypalClient;
        private readonly ItalianCharmBraceletContext _context;
        private readonly IVnPayService _vnPayService;

        public CartController(ItalianCharmBraceletContext context, PaypalClient paypalClient, IVnPayService vnPayService)
        {
            _paypalClient = paypalClient;
            _context = context;
            _vnPayService = vnPayService;
        }

        public List<CartItemVM> Cart => HttpContext.Session.Get<List<CartItemVM>>(MySetting.CART_KEY) ?? new List<CartItemVM>();

        public IActionResult Index()
        {
            var greetings = new List<string>
            {
                "Your cart's filling up with joy! 🛒💖 Let’s see what treasures you've picked!",
                "Oh, the excitement! 🎉 Let’s check out your fabulous finds!",
                "Your cart looks amazing! 🛍️💎 Ready to make these yours?",
                "Yay! Your cart is one step closer to happiness! 🎈💖 Let’s peek inside!",
                "Woohoo! 🎉 Your cart is looking good! Let’s bring these goodies home!",
                "Your picks are shining in the cart! ✨💎 Can’t wait to see what you’ll take home!",
                "The perfect collection awaits! 💖🛒 Ready to make it yours?",
                "Your shopping spree just got even better! 🌈🎁 Let’s take a look inside!",
                "OMG, hurry up! These goodies can’t wait to be yours! 🎉💖 They’re practically jumping to join you!"
            };

            var random = new Random();
            var randomGreeting = greetings[random.Next(greetings.Count)];

            // Truyền câu ngẫu nhiên vào View thông qua ViewData
            ViewData["RandomGreetingCart"] = randomGreeting;
            return View(Cart);
        }

        [HttpPost]
        public IActionResult UpdateCart(string id, int quantity = 1)
        {
            var gioHang = Cart;
            var item = gioHang.SingleOrDefault(p => p.CharmId == id);
            if (item == null)
            {
                var hangHoa = _context.Charms.SingleOrDefault(p => p.Id == id);
                if (hangHoa == null)
                {
                    TempData["Message"] = "Không tìm thấy hàng hóa";
                    return NotFound();
                }
                gioHang.Add(new CartItemVM()
                {
                    CharmId = hangHoa.Id,
                    Name = hangHoa.Name,
                    Price = hangHoa.Price ?? 0,
                    Img = hangHoa.Img ?? "",
                    Quantity = quantity,
                    CateId = hangHoa.CateId
                });
            }
            else
            {
                item.Quantity += quantity;
                if (item.Quantity <= 0)
                {
                    return Json(new
                    {
                        success = false,
                        remove = true,
                    });
                }
            }
            HttpContext.Session.Set(MySetting.CART_KEY, gioHang);
            //return RedirectToAction("Index");
            return Json(new
            {
                success = true,
                message = "Sản phẩm đã được thêm vào giỏ hàng",
                total = gioHang.SingleOrDefault(p => p.CharmId == id).Total,
                gioHang = new { quantity = gioHang.Sum(p => p.Quantity) }
            });
        }

        public IActionResult RemoveFormCart(string id)
        {
            var gioHang = Cart;
            var item = gioHang.SingleOrDefault(p => p.CharmId == id);
            if (item != null)
            {
                gioHang.Remove(item);
                HttpContext.Session.Set(MySetting.CART_KEY, gioHang);
            }
            return Json(new
            {
                success = true,
                message = "Sản phẩm đã được xóa khỏi giỏ hàng",
                gioHang = new { quantity = gioHang.Sum(p => p.Quantity) }
            });
        }

        #region Checkout
        [Authorize]
        [HttpGet]
        public IActionResult Checkout()
        {
            if (Cart.Count == 0)
            {
                return RedirectToAction("Index");
            }
            ViewBag.PaypalClientId = _paypalClient.ClientId;
            return View(Cart);
        }

        [Authorize]
        [HttpPost]
        public IActionResult Checkout(CheckoutVM model)
        {
            if (ModelState.IsValid)
            {
                var customerId = HttpContext.User.Claims.SingleOrDefault(p => p.Type == MySetting.CLAIM_CUSTOMER).Value;
                var hoadon = new SalesInvoice()
                {
                    Id = Util.GenerateID(_context, "HDB"),
                    CustomerId = customerId,
                    Name = model.Name,
                    Address = model.Address,
                    Cell = model.Cell,
                    Note = model.Note,
                    Date = DateOnly.FromDateTime(DateTime.Now),
                    StateId = "0",
                    PaymentMethod = model.PaymentMethod,
                    TotalPayment = Cart.Sum(p => p.Total)
                };

                var success = UpdateDatebase(hoadon);
                if (success)
                {
                    HttpContext.Session.Set<List<CartItemVM>>(MySetting.CART_KEY, new List<CartItemVM>());
                    //HttpContext.Session.Remove(MySetting.CART_KEY);
                    //return RedirectToAction("PaymentSuccess");
                    return Json(new { success = true, message = "" });
                }
            }
            return PartialView("FormCheckout", model);
            //return View(Cart);
        }
        #endregion

        public bool UpdateDatebase(SalesInvoice hoadon)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    _context.Add(hoadon);
                    _context.SaveChanges();
                    var cthds = new List<SalesInvoiceDetail>();
                    foreach (var item in Cart)
                    {
                        cthds.Add(new SalesInvoiceDetail()
                        {
                            InvoiceId = hoadon.Id,
                            ProductId = item.CharmId,
                            Quantity = item.Quantity,
                            Price = item.Price,
                            Note = "",
                        });
                    }
                    _context.AddRange(cthds);
                    _context.SaveChanges();
                    transaction.Commit(); //sua theo chatgpt
                    return true;
                }
                catch
                {
                    transaction.Rollback();
                    return false;
                }
            }
        }

        #region Paypal
        [Authorize]
        [HttpPost("/Cart/create-paypal-order")]
        public async Task<IActionResult> CreatePaypalOrder(CancellationToken cancellationToken)
        {
            var value = Cart.Sum(p => p.Total).ToString();
            var currency = "USD";
            var madonhangthamchieu = "DH" + DateTime.Now.Ticks.ToString();

            var jsonResponse = await _paypalClient.CreateOrder(value, currency, madonhangthamchieu);
            if (jsonResponse != null)
            {
                var orderId = jsonResponse["id"]?.ToString() ?? "";
                return new JsonResult(new { Id = orderId });
            }

            return new JsonResult(new { Id = "" });
        }

        [Authorize]
        [HttpPost("/Cart/capture-paypal-order")]
        public async Task<IActionResult> CapturePaypalOrder([FromBody] Dictionary<string, string> formData)
        {
            if (formData != null)
            {
                string paymentMethod = formData["PaymentMethod"];
                string orderId = formData["OrderId"];

                var jsonResponse = await _paypalClient.CaptureOrder(orderId);

                if (jsonResponse != null)
                {
                    string paypalOrderStatus = jsonResponse["status"]?.ToString() ?? "";
                    if (paypalOrderStatus == "COMPLETED")
                    {
                        var customerId = HttpContext.User.Claims.SingleOrDefault(p => p.Type == MySetting.CLAIM_CUSTOMER).Value;
                        var hoadon = new SalesInvoice()
                        {
                            Id = Util.GenerateID(_context, "HDB"),
                            CustomerId = customerId,
                            Name = formData["Name"],
                            Address = formData["Address"],
                            Cell = formData["Cell"],
                            Note = formData["Note"],
                            Date = DateOnly.FromDateTime(DateTime.Now),
                            StateId = "0",
                            PaymentMethod = paymentMethod,
                            TotalPayment = Cart.Sum(p => p.Total)
                        };

                        var success = UpdateDatebase(hoadon);
                        if (success)
                        {
                            HttpContext.Session.Set<List<CartItemVM>>(MySetting.CART_KEY, new List<CartItemVM>());
                            //HttpContext.Session.Remove(MySetting.CART_KEY);
                            return new JsonResult("success");
                        }
                        return new JsonResult("error but completed");
                    }
                }
            }
            return new JsonResult("error");
        }


        //public async Task<IActionResult> CreatePaypalOrder(CancellationToken cancellationToken)
        //{
        //    //Thông tin đơn hàng gửi qua paypal
        //    var tongtien = Cart.Sum(p => p.Total).ToString();
        //    var donvitiente = "USD";
        //    var madonhangthamchieu = "DH" + DateTime.Now.Ticks.ToString();
        //    try
        //    {
        //        var respone = await _paypalClient.CreateOrder(tongtien, donvitiente, madonhangthamchieu);
        //        return Ok(respone);
        //    }
        //    catch (Exception ex)
        //    {
        //        var error = new { ex.GetBaseException().Message };
        //        return BadRequest(error);
        //    }
        //}

        //public async Task<IActionResult> CapturePaypalOrder(string orderId, CancellationToken cancellationToken)
        //{
        //    try
        //    {
        //        var respone = await _paypalClient.CaptureOrder(orderId);
        //        //Lưu database
        //        return Ok(respone);
        //    }
        //    catch (Exception ex)
        //    {
        //        var error = new { ex.GetBaseException().Message };
        //        return BadRequest(error);
        //    }
        //}

        #endregion

        public IActionResult PaymentSuccess()
        {
            return View("Success");
        }

        [Authorize]
        public IActionResult PaymentCallBack()
        {
            return View();
        }

    }
}
