using AutoMapper;
using ItalianCharmBracelet.Areas.Admin.ViewModels;
using ItalianCharmBracelet.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ItalianCharmBracelet.Areas.Admin.Controllers
{

    [Area("Admin")]
    [Route("Admin")]
    [Route("Admin/Home")]
    //[Authorize(Roles = "1")]
    public class HomeController : Controller
    {
        private readonly ItalianCharmBraceletContext _context;

        public HomeController(ItalianCharmBraceletContext context)
        {
            _context = context;
        }

        [Route("Index")]
        [Route("")]
        public IActionResult Index()
        {
            DateOnly start_month = new DateOnly(DateTime.Now.Year, DateTime.Now.Month, 1);
            DateOnly start_year = new DateOnly(DateTime.Now.Year, 1, 1);
            DateOnly end = DateOnly.FromDateTime(DateTime.Now);
            ViewBag.Earnings_Month = _context.SalesInvoices.Where(h => h.Date >= start_month && h.Date <= end).Sum(x => x.TotalPayment);
            ViewBag.Earnings_Year = _context.SalesInvoices.Where(h => h.Date >= start_year && h.Date <= end).Sum(x => x.TotalPayment);

            var monthNames = new[] { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };
            var salesData = _context.SalesInvoices.Where(h => h.Date >= start_year && h.Date <= end)
                                               .GroupBy(h => new { h.Date.Value.Month })
                                               .Select(g => new 
                                               {
                                                   Month = monthNames[g.Key.Month - 1],
                                                   Total = g.Sum(h => h.TotalPayment),
                                               })
                                               .ToList();

            ViewData["SalesData"] = JsonConvert.SerializeObject(salesData);

            //ViewBag.SalesData = new List<object>
            //{
            //    new { Month = "January", Total = 100 },
            //    new { Month = "February", Total = 200 },
            //    new { Month = "March", Total = 150 }
            //};

            return View();
        }

    }
}
