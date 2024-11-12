﻿using Microsoft.AspNetCore.Mvc;
using ItalianCharmBracelet.Data;
using ItalianCharmBracelet.ViewModels;
using ItalianCharmBracelet.Helpers;

namespace ItalianCharmBracelet.Controllers
{
    public class CartController : Controller
    {
        private readonly ItalianCharmBraceletContext _context;
        public CartController(ItalianCharmBraceletContext context)
        {
            _context = context;
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
                        remove =true,
                    });
                }
            }
            HttpContext.Session.Set(MySetting.CART_KEY, gioHang);
            //return RedirectToAction("Index");
            return Json(new { success = true,
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
    }
}