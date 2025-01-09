using ItalianCharmBracelet.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using X.PagedList;
using Microsoft.AspNetCore.Mvc.Rendering;
using ItalianCharmBracelet.Helpers;

namespace ItalianCharmBracelet.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin")]
    [Route("Admin/Product")]
    public class ProductController : Controller
    {
        private readonly ItalianCharmBraceletContext _context;
        public ProductController(ItalianCharmBraceletContext context)
        {
            _context = context;
        }

        [Route("")]
        public IActionResult Product(string? cate, int? page)
        {
            int pageSize = 20;
            int pageNumber = page == null || page < 0 ? 1 : page.Value;

            var charms = _context.Charms.AsQueryable();
            charms = charms.Where(p => p.State == "1");
            if (!string.IsNullOrEmpty(cate))
            {
                charms = charms.Where(p => p.CateId == cate);
            }
            PagedList<Charm> list = new PagedList<Charm>(charms, pageNumber, pageSize);
            ViewData["cate"] = cate;
            return View(list);
        }

        [Route("Bin")]
        public IActionResult DeletedProduct(string? cate, int? page)
        {
            int pageSize = 20;
            int pageNumber = page == null || page < 0 ? 1 : page.Value;

            var charms = _context.Charms.AsQueryable();
            charms = charms.Where(p => p.State == "0");
            if (!string.IsNullOrEmpty(cate))
            {
                charms = charms.Where(p => p.CateId == cate);
            }
            PagedList<Charm> list = new PagedList<Charm>(charms, pageNumber, pageSize);
            ViewData["cate"] = cate;
            return View(list);
        }

        [Route("Add")]
        [HttpGet]
        public IActionResult AddProduct()
        {
            ViewBag.CateId = new SelectList(_context.Categories, "Id", "Name");
            var charmGroups = _context.Charms
                            .GroupBy(c => c.Id.Substring(0, 7))
                            .Select(g => g.Key)
                            .ToList();
            ViewBag.ProId = new SelectList(charmGroups);
            return View();
        }

        [Route("Add")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddProduct(Charm model)
        {
            if (ModelState.IsValid)
            {
                //var charm = new Charm();
                //charm = Mapper.Map<CharmViewModel, Charm>(model);
                model.Id = Util.GenerateID(_context, model.Id);
                _context.Charms.Add(model);
                _context.SaveChanges();
                return RedirectToAction("Product");
            }
            ViewBag.CateId = new SelectList(_context.Categories, "Id", "Name");
            var charmGroups = _context.Charms
                            .GroupBy(c => c.Id.Substring(0, 7))
                            .Select(g => g.Key)
                            .ToList();
            ViewBag.ProId = new SelectList(charmGroups);
            return View(model);
        }

        [Route("Update")]
        [HttpGet]
        public IActionResult UpdateProduct(string CharmId, string option = "1")
        {
            var charm = _context.Charms.Find(CharmId);
            if (option == "2")
            {
                charm.State = "0";
                _context.Entry(charm).State = EntityState.Modified;
                _context.SaveChanges();
                return RedirectToAction("Product");
            }
            ViewBag.CateId = new SelectList(_context.Categories, "Id", "Name");
            var charmGroups = _context.Charms
                            .GroupBy(c => c.Id.Substring(0, 7))
                            .Select(g => g.Key)
                            .ToList();
            ViewBag.ProId = new SelectList(charmGroups);
            return View(charm);
        }

        [Route("Update")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateProduct(Charm model)
        {
            if (ModelState.IsValid)
            {
                _context.Entry(model).State = EntityState.Modified;
                _context.SaveChanges();
                return RedirectToAction("Product");
            }
            ViewBag.CateId = new SelectList(_context.Categories, "Id", "Name");
            var charmGroups = _context.Charms
                            .GroupBy(c => c.Id.Substring(0, 7))
                            .Select(g => g.Key)
                            .ToList();
            ViewBag.ProId = new SelectList(charmGroups);
            return View(model);
        }

        [Route("Delete")]
        public IActionResult DeleteProduct(string CharmId)
        {
            TempData["Message"] = "";
            var charm = _context.Charms.Find(CharmId);
            if (charm == null)
            {
                TempData["Message"] = "Không tìm thấy sản phẩm";
                return RedirectToAction("Product");
            }
            _context.Charms.Remove(charm);
            _context.SaveChanges();
            TempData["Message"] = "Xóa sản phẩm thành công";
            return RedirectToAction("Product");
        }
    }
}
