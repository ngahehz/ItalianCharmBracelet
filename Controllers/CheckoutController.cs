using ItalianCharmBracelet.Data;
using ItalianCharmBracelet.Helpers;
using ItalianCharmBracelet.Services;
using ItalianCharmBracelet.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ItalianCharmBracelet.Controllers
{
    [Authorize]
    public class CheckoutController : Controller
    {
        //public IActionResult Index()
        //{
        //    return View();
        //}
        private readonly PaypalClient _paypalClient;
        private readonly ItalianCharmBraceletContext _context;
        private readonly IVnPayService _vnPayService;

        public CheckoutController(ItalianCharmBraceletContext context, PaypalClient paypalClient, IVnPayService vnPayService)
        {
            _paypalClient = paypalClient;
            _context = context;
            _vnPayService = vnPayService;
        }
        public List<CartItemVM> Cart => HttpContext.Session.Get<List<CartItemVM>>(MySetting.CART_KEY) ?? new List<CartItemVM>();


        [HttpGet]
        public IActionResult Index()
        {
            var greetings = new List<string>
            {
                "You're one step away from happiness! 🛍️✨",
                "Securely completing your payment... 💳🔒",
                "Let’s wrap this up! 🎁 Your order is almost on its way!",
                "Shopping made easy! 🛒💖 Just fill in your details and you're done!",
                "You're about to make your day brighter! ☀️💳",
                "Your payment is 100% secure with us! 🛡️✨",
                "We value your trust! 🔒💙 Pay securely and confidently!",
                "Final stretch! 🏁✨ Your goodies are almost yours!",
                "We can't wait to pack your order! 📦✨"
            };

            var random = new Random();
            var randomGreeting = greetings[random.Next(greetings.Count)];

            ViewData["RandomGreetingCheckout"] = randomGreeting;

            //string code = HttpContext.Session.GetString("InputData");
            //ViewBag.InputData = code;

            if (Cart.Count == 0)
            {
                return RedirectToAction("Index", "Cart");
            }
            ViewBag.PaypalClientId = _paypalClient.ClientId;
            return View(Cart);
        }

        [Authorize]
        [HttpPost]
        public IActionResult Index(CheckoutVM model)
        {
            if (ModelState.IsValid)
            {
                string code = HttpContext.Session.GetString("InputData");
                var voucher = _context.Vouchers.SingleOrDefault(p => p.Code == code);
                var totalPayment = Cart.Sum(p => p.Total);
                double discount = CheckDiscount();
                if (discount == 0)
                {
                    ModelState.AddModelError("", "Số lượng voucher đã hết");
                    return PartialView("FormCheckout", model);
                }
                if (discount == -1)
                    discount = 0;

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
                    VoucherId = voucher?.Id,
                    PaymentMethod = model.PaymentMethod,
                    TotalPayment = totalPayment - discount,
                };

                if (model.PaymentMethod == "VnPay")
                {
                    HttpContext.Session.Set("HOADONVNPAY", hoadon);
                    var vnpayModel = new VnpaymentRequestModel
                    {
                        Amount = totalPayment - discount,
                        CreatedDate = DateTime.Now,
                        Description = $"{model.Cell}",
                        FullName = model.Name,
                        OrderId = hoadon.Id,
                    };
                    var paymentUrl = _vnPayService.CreatePaymentUrl(HttpContext, vnpayModel);
                    return Json(new { success = true, redirectUrl = paymentUrl });
                }

                var success = UpdateDatebase(hoadon);
                if (success)
                {
                    HttpContext.Session.Set<List<CartItemVM>>(MySetting.CART_KEY, new List<CartItemVM>());
                    HttpContext.Session.SetString("InputData", "");
                    //HttpContext.Session.Remove(MySetting.CART_KEY);
                    //return RedirectToAction("PaymentSuccess");
                    return Json(new { success = true, redirectUrl = "/Checkout/PaymentSuccess" });
                }

            }
            return PartialView("FormCheckout", model);
            //return View(Cart);
        }

        public IActionResult CheckVoucherAvailability()
        {
            double discount = CheckDiscount();
            if (discount == -1)
                return Json(new
                {
                    success = false,
                    message = "Không nhập voucher",
                });
            if (discount == 0)
                return Json(new
                {
                    success = false,
                    message = "Số lượng voucher đã hết",
                });
            return Json(new
            {
                success = true,
                message = "Voucher hợp lệ",
            });
        }

        public double CheckDiscount()
        {
            string code = HttpContext.Session.GetString("InputData");
            double totalPayment = Cart.Sum(p => p.Total);

            var voucher = _context.Vouchers.SingleOrDefault(p => p.Code == code);

            if (voucher != null)
            {
                if (voucher.State == 0)
                    return 0;
                if (voucher.DiscountAmount != null)
                    return (double)voucher.DiscountAmount;
                else
                    return (double)(totalPayment * voucher.PercentDiscount > voucher.MaxDiscount ? voucher.MaxDiscount : totalPayment * voucher.PercentDiscount);
            }
            return -1;
        }

        public bool UpdateDatebase(SalesInvoice hoadon)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    _context.Add(hoadon);
                    //_context.SaveChanges();
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

                        var charm = _context.Charms.Find(item.CharmId);
                        charm.Quantity -= item.Quantity;
                        _context.Entry(charm).State = EntityState.Modified;
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
        [HttpPost("/Checkout/create-paypal-order")]
        public async Task<IActionResult> CreatePaypalOrder(CancellationToken cancellationToken)
        {
            var totalPayment = Cart.Sum(p => p.Total);
            double discount = CheckDiscount();
            if (discount == 0)
            {
                //ModelState.AddModelError("", "Lỗi hệ thống");
                return new JsonResult(new { Id = "" });
            }
            if (discount == -1)
                discount = 0;
            const double VndToUsdRate = 24000;
            var totalInUsd = Math.Round((totalPayment - discount) / VndToUsdRate, 2);

            var value = totalInUsd.ToString();
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
        [HttpPost("/Checkout/capture-paypal-order")]
        public async Task<IActionResult> CapturePaypalOrder([FromBody] Dictionary<string, string> formData)
        {
            if (formData != null)
            {
                try
                {
                    string paymentMethod = formData["PaymentMethod"];
                    string orderId = formData["OrderId"];

                    var jsonResponse = await _paypalClient.CaptureOrder(orderId);

                    if (jsonResponse != null)
                    {
                        string paypalOrderStatus = jsonResponse["status"]?.ToString() ?? "";
                        if (paypalOrderStatus == "COMPLETED")
                        {
                            string code = HttpContext.Session.GetString("InputData");
                            var voucher = _context.Vouchers.SingleOrDefault(p => p.Code == code);
                            var totalPayment = Cart.Sum(p => p.Total);
                            double discount = CheckDiscount();
                            if (discount == 0)
                            {
                                return new JsonResult("error");
                            }
                            if (discount == -1)
                                discount = 0;

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
                                VoucherId = voucher?.Id,
                                PaymentMethod = paymentMethod,
                                TotalPayment = totalPayment - discount,
                            };

                            var success = UpdateDatebase(hoadon);
                            if (success)
                            {
                                HttpContext.Session.Set<List<CartItemVM>>(MySetting.CART_KEY, new List<CartItemVM>());
                                HttpContext.Session.SetString("InputData", "");
                                //HttpContext.Session.Remove(MySetting.CART_KEY);
                                return new JsonResult("success");
                            }
                            return new JsonResult("error but completed");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"Lỗi trong CreatePaypalOrder: {ex.Message}");
                    return new JsonResult(new { Id = "", Message = "Đã xảy ra lỗi trong quá trình xử lý. Vui lòng thử lại sau." });
                }
            }
            return new JsonResult("error");
        }

        #endregion

        #region VnPay
        [Authorize]
        public IActionResult PaymentCallBack()
        {
            var response = _vnPayService.PaymentExecute(Request.Query);
            if (response == null || response.VnPayResponseCode != "00")
            {
                string text = $"Lỗi thanh toán VN Pay: {response?.VnPayResponseCode ?? "Không xác định"}";
                return RedirectToAction("PaymentFailure", new { text = text });
            }

            var hoadon = HttpContext.Session.Get<SalesInvoice>("HOADONVNPAY");
            var success = UpdateDatebase(hoadon);
            if (!success)
            {
                return RedirectToAction("PaymentFailure", "Liên hệ admin để được xử lý");
            }
            HttpContext.Session.Set<List<CartItemVM>>(MySetting.CART_KEY, new List<CartItemVM>());
            return RedirectToAction("PaymentSuccess");
        }
        #endregion

        public IActionResult PaymentSuccess()
        {
            return View("Success");
        }

        public IActionResult PaymentFailure(string text)
        {
            ViewBag.ErrorMessage = text ?? "Đã xảy ra lỗi trong quá trình thanh toán.";
            return View("Failure");
        }





    }
}
