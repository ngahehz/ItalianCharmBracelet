using ItalianCharmBracelet.Data;
using Microsoft.AspNetCore.Mvc;
using X.PagedList;

namespace ItalianCharmBracelet.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin")]
    [Route("Admin/Invoice")]
    public class InvoiceController : Controller
    {
        private readonly ItalianCharmBraceletContext _context;
        public InvoiceController(ItalianCharmBraceletContext context)
        {
            _context = context;
        }

        [Route("PurchaseInvoice")]
        public IActionResult PurchaseInvoice(int? page)
        {
            int pageSize = 20;
            int pageNumber = page == null || page < 0 ? 1 : page.Value;

            var invoices = _context.PurchaseInvoices.AsQueryable();
            PagedList<PurchaseInvoice> list = new PagedList<PurchaseInvoice>(invoices, pageNumber, pageSize);
            return View(list);
        }

        [Route("SalesInvoice")]
        public IActionResult SalesInvoice(int? page)
        {
            int pageSize = 20;
            int pageNumber = page == null || page < 0 ? 1 : page.Value;

            var invoices = _context.SalesInvoices.AsQueryable();
            PagedList<SalesInvoice> list = new PagedList<SalesInvoice>(invoices, pageNumber, pageSize);
            return View(list);
        }

        [Route("State")]
        public IActionResult State()
        {
            var states = _context.States.AsQueryable();
            return View(states);
        }
    }
}
