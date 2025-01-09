using ItalianCharmBracelet.Data;
using ItalianCharmBracelet.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

namespace ItalianCharmBracelet.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin")]
    [Route("Admin/Voucher")]
    public class VoucherController : Controller
    {
        private readonly ItalianCharmBraceletContext _context;
        public VoucherController(ItalianCharmBraceletContext context)
        {
            _context = context;
        }

        [Route("")]
        public IActionResult Voucher(int? page)
        {
            int pageSize = 20;
            int pageNumber = page == null || page < 0 ? 1 : page.Value;

            var vouchers = _context.Vouchers.AsQueryable();
            vouchers = vouchers.Where(p => p.State > 0);
            PagedList<Voucher> list = new PagedList<Voucher>(vouchers, pageNumber, pageSize);
            return View(list);
        }

        #region add
        [Route("Add")]
        [HttpGet]
        public IActionResult AddVoucher()
        {
            ViewBag.VcId = Util.GenerateID(_context, "VC");
            return View();
        }

        [Route("Add")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddVoucher(Voucher model)
        {
            if (ModelState.IsValid)
            {
                _context.Vouchers.Add(model);
                _context.SaveChanges();
                return RedirectToAction("Voucher");
            }
            ViewBag.VcId = Util.GenerateID(_context, "VC");
            return View(model);
        }
        #endregion

        [Route("Bin")]
        public IActionResult DeletedVoucher(int? page)
        {
            int pageSize = 20;
            int pageNumber = page == null || page < 0 ? 1 : page.Value;

            var vouchers = _context.Vouchers.AsQueryable();
            vouchers = vouchers.Where(p => p.State == -1);
            PagedList<Voucher> list = new PagedList<Voucher>(vouchers, pageNumber, pageSize);
            return View(list);
        }

        [Route("Update")]
        [HttpGet]
        public IActionResult UpdateVoucher(string VoucherId, string option = "1")
        {
            var voucher = _context.Charms.Find(VoucherId);
            if (option == "2")
            {
                voucher.State = "0";
                _context.Entry(voucher).State = EntityState.Modified;
                _context.SaveChanges();
                return RedirectToAction("Voucher");
            }
            return View(voucher);
        }

        [Route("Update")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateVoucher(Voucher model)
        {
            if (ModelState.IsValid)
            {
                _context.Entry(model).State = EntityState.Modified;
                _context.SaveChanges();
                return RedirectToAction("Voucher");
            }
            return View(model);
        }
    }
}
