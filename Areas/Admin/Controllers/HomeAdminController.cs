using AutoMapper;
using ItalianCharmBracelet.Data;
using Microsoft.AspNetCore.Mvc;

namespace ItalianCharmBracelet.Areas.Admin.Controllers
{
    [Area("admin")]
    [Route("admin")]
    [Route("admin/homeadmin")]
    public class HomeAdminController : Controller
    {
        private readonly ItalianCharmBraceletContext _context;

        public HomeAdminController(ItalianCharmBraceletContext context, IMapper mapper)
        {
            _context = context;
        }

        [Route("index")]
        [Route("")]
        public IActionResult Index()
        {
            return View();
        }
        [Route("productmanagement")]
        public IActionResult ProductManagement()
        {
            var list = _context.Charms.ToList();
            return View(list);
        }
    }
}
