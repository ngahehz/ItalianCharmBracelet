using ItalianCharmBracelet.Data;
using ItalianCharmBracelet.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace ItalianCharmBracelet.ViewComponents
{
    public class CategoryViewComponent : ViewComponent
    {
        private readonly ItalianCharmBraceletContext _context;

        public CategoryViewComponent(ItalianCharmBraceletContext context)
        {
            _context = context;
        }

        public IViewComponentResult Invoke()
        {
            var categories = _context.Categories.Select(lo => new CategoryVM
            {
                Id = lo.Id,
                Name = lo.Name,
                Quantity = lo.Charms.Count
            }).OrderBy(p => p.Id);

            return View(categories);
        }
    }
}
