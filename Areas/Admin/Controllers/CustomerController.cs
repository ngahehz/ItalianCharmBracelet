using ItalianCharmBracelet.Data;
using Microsoft.AspNetCore.Mvc;
using X.PagedList;

namespace ItalianCharmBracelet.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin")]
    [Route("Admin/Customer")]
    public class CustomerController : Controller
    {
        private readonly ItalianCharmBraceletContext _context;
        public CustomerController(ItalianCharmBraceletContext context)
        {
            _context = context;
        }

        [Route("")]
        public IActionResult Customer(int? page)
        {
            int pageSize = 20;
            int pageNumber = page == null || page < 0 ? 1 : page.Value;

            var customers = _context.Customers.AsQueryable();
            customers = customers.Where(p => p.State == "1");
            PagedList<Customer> list = new PagedList<Customer>(customers, pageNumber, pageSize);
            return View(list);
        }
    }
}
