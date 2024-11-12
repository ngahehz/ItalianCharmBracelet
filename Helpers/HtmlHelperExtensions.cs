using Microsoft.AspNetCore.Mvc.Rendering;

namespace ItalianCharmBracelet.Helpers
{
    public static class HtmlHelperExtensions
    {
        public static string IsActive(this IHtmlHelper htmlHelper, string controller, string action)
        {
            var routeData = htmlHelper.ViewContext.RouteData;
            var routeAction = routeData.Values["Action"]?.ToString();
            var routeController = routeData.Values["Controller"]?.ToString();

            return controller == routeController && action == routeAction ? "active" : "";
        }
    }
}
