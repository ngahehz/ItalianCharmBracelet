using ItalianCharmBracelet.Data;
using Microsoft.AspNetCore.Mvc;
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
            vouchers = vouchers.Where(p => p.State == "1");
            PagedList<Voucher> list = new PagedList<Voucher>(vouchers, pageNumber, pageSize);
            return View(list);
        }
    }
}
