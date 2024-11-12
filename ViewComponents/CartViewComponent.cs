using ItalianCharmBracelet.Helpers;
using ItalianCharmBracelet.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace ItalianCharmBracelet.ViewComponents
{
    public class CartViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            var cart = HttpContext.Session.Get<List<CartItemVM>>(MySetting.CART_KEY) ?? new List<CartItemVM>();
            return View("CartPanel", new CartVM
            {
                Quantity = cart.Sum(p => p.Quantity),
                Total = cart.Sum(p => p.Quantity * p.Price)
            });
        }
    }
}
