using ItalianCharmBracelet.Data;
using ItalianCharmBracelet.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

namespace ItalianCharmBracelet.Controllers
{
    public class ProductController : Controller
    {
        private readonly ItalianCharmBraceletContext _context;
        public ProductController(ItalianCharmBraceletContext context)
        {
            _context = context;
        }
        public IActionResult Index(string? cate, int? page)
        {
            int pageSize = 18;
            int pageNumber = page == null || page < 0 ? 1 : page.Value;

            var charms = _context.Charms.AsQueryable();
            if (!string.IsNullOrEmpty(cate))
            {
                charms = charms.Where(p => p.CateId == cate);
            }
            var result = charms.Select(p => new InfoProductVM(p.Id, p.Name, p.Price ?? 0, p.Img ?? "", p.CateId ?? "", p.Cate.Name));
            PagedList<InfoProductVM> list = new PagedList<InfoProductVM>(result, pageNumber, pageSize);
            ViewData["cate"] = cate;
            return View(list);
        }

        public IActionResult Search(string? query)
        {
            var hangHoas = _context.Charms.AsQueryable();
            if (query != null)
            {
                hangHoas = hangHoas.Where(p => p.Name.Contains(query));
            }
            var result = hangHoas.Select(p => new InfoProductVM(p.Id, p.Name, p.Price ?? 0, p.Img ?? "", p.CateId ?? "", p.Cate.Name));
            return View(result);
        }

        public IActionResult Detail(string id)
        {
            var greetings = new List<string>
            {
                "Yay! 🎉 We're thrilled you're here! This little gem can’t wait to show off its charm just for you!",
                "Woohoo! 🎉 You've just unlocked all the magic this product has to offer! We hope it’s love at first sight! 💖",
                "You’ve got great taste! 😍 Let’s dive into all the fabulous details together!",
                "We knew you’d be curious! 💫 This product is ready to show off its best for you!",
                "Oh, we’re excited you’re here! 🌈 Prepare to be dazzled by all the amazing details!",
                "Eep! 🎀 So happy you're taking a peek! Hope it’s as delightful as we think it is!",
                "A closer look? Yes, please! 🌟 This beauty has been waiting just for you—enjoy every detail!",
                "You’ve just taken the first step toward something wonderful! 🌟 Let's explore every sparkle together!"
            };

            var random = new Random();
            var randomGreeting = greetings[random.Next(greetings.Count)];

            // Truyền câu ngẫu nhiên vào View thông qua ViewData
            ViewData["RandomGreeting"] = randomGreeting;

            var charm = _context.Charms
                .Include(p => p.Cate)
                .SingleOrDefault(p => p.Id == id);
            if (charm == null)
            {
                TempData["Message"] = "Không tìm thấy hàng hóa";
                return NotFound();
            }
            var result = new InfoProductVM(charm.Id, charm.Name, charm.Quantity ?? 0, charm.Price ?? 0, charm.Img ?? "", charm.Description ?? "", charm.CateId ?? "", charm.Cate.Name);
            return View(result);
        }
    }
}
