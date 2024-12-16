using ItalianCharmBracelet.Data;
using Microsoft.AspNetCore.Mvc;
using System.Drawing.Printing;
using System;
using X.PagedList;

namespace ItalianCharmBracelet.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin")]
    [Route("Admin/Category")]
    public class CategoryController : Controller
    {
        private readonly ItalianCharmBraceletContext _context;
        public CategoryController(ItalianCharmBraceletContext context)
        {
            _context = context;
        }

        [Route("")]
        public IActionResult Category()
        {
            var categorys = _context.Categories.AsQueryable();
            return View(categorys);
        }

    }
}
