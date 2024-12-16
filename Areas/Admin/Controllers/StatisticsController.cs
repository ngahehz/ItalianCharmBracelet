using ItalianCharmBracelet.Areas.Admin.ViewModels;
using ItalianCharmBracelet.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Globalization;

namespace ItalianCharmBracelet.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin")]
    [Route("Admin/Statistics")]
    public class StatisticsController : Controller
    {
        private readonly ItalianCharmBraceletContext _context;
        public StatisticsController(ItalianCharmBraceletContext context)
        {
            _context = context;
        }

        [Route("Finance")]
        public IActionResult Finance(string startDate, string endDate)
        {
            DateOnly start = string.IsNullOrEmpty(startDate) ? DateOnly.FromDateTime(new DateTime(2024, 1, 1)) : DateOnly.ParseExact(startDate, "yyyy-MM", CultureInfo.InvariantCulture);
            int year, month;
            DateOnly end = string.IsNullOrEmpty(endDate) ? DateOnly.FromDateTime(DateTime.Now) : new DateOnly(
                                                                                                        year = DateOnly.ParseExact(endDate, "yyyy-MM", CultureInfo.InvariantCulture).Year,
                                                                                                        month = DateOnly.ParseExact(endDate, "yyyy-MM", CultureInfo.InvariantCulture).Month,
                                                                                                        DateTime.DaysInMonth(year, month));

            var Income = _context.SalesInvoices.Where(h => h.Date >= start && h.Date <= end)
                                                  .GroupBy(h => new { h.Date.Value.Year, h.Date.Value.Month })
                                                  .Select(g => new FinanceVM
                                                  {
                                                      Month = g.Key.Month.ToString("00") + "/" + g.Key.Year,
                                                      Income = g.Sum(h => h.TotalPayment),
                                                      Expenses = 0
                                                  })
                                                  .ToList();

            var Expenses = _context.PurchaseInvoices.Where(h => h.Date >= start && h.Date <= end)
                                                  .GroupBy(h => new { h.Date.Value.Year, h.Date.Value.Month })
                                                  .Select(g => new FinanceVM
                                                  {
                                                      Month = g.Key.Month.ToString("00") + "/" + g.Key.Year,
                                                      Income = 0,
                                                      Expenses = g.Sum(h => h.TotalPayment)
                                                  })
                                                  .ToList();

            var allData = Income.Union(Expenses)
                                .GroupBy(t => t.Month)
                                .Select(g => new FinanceVM
                                {
                                    Month = g.Key,
                                    Income = g.Sum(t => t.Income),
                                    Expenses = g.Sum(t => t.Expenses),
                                    Balance = g.Sum(t => t.Income) - g.Sum(t => t.Expenses)
                                })
                                .OrderBy(t => t.Month)
                                .ToList();

            ViewBag.StartDate = start.ToString("yyyy-MM");
            ViewBag.EndDate = end.ToString("yyyy-MM");

            return View(allData);
        }

        [Route("Products")]
        public IActionResult Products(string startDate, string endDate)
        {
            DateOnly start = string.IsNullOrEmpty(startDate) ? DateOnly.FromDateTime(new DateTime(2024, 1, 1)) : DateOnly.ParseExact(startDate, "yyyy-MM", CultureInfo.InvariantCulture);
            int year, month;
            DateOnly end = string.IsNullOrEmpty(endDate) ? DateOnly.FromDateTime(DateTime.Now) : new DateOnly(
                                                                                                        year = DateOnly.ParseExact(endDate, "yyyy-MM", CultureInfo.InvariantCulture).Year,
                                                                                                        month = DateOnly.ParseExact(endDate, "yyyy-MM", CultureInfo.InvariantCulture).Month,
                                                                                                        DateTime.DaysInMonth(year, month));
            var report = (from invoice in _context.SalesInvoices
                          join detail in _context.SalesInvoiceDetails
                          on invoice.Id equals detail.InvoiceId
                          where invoice.Date >= start && invoice.Date <= end
                          group new { invoice, detail } by new
                          {
                              Year = invoice.Date.Value.Year,
                              Month = invoice.Date.Value.Month,
                              detail.ProductId,
                              detail.Product.Name
                          } into groupedData
                          select new ProductsVM
                          {
                              Month = groupedData.Key.Month.ToString("00") + "/" + groupedData.Key.Year,
                              Id = groupedData.Key.ProductId,
                              Name = groupedData.Key.Name,
                              Quantity = groupedData.Sum(x => x.detail.Quantity),
                              Revenue = groupedData.Sum(x => x.detail.Price)
                          }).ToList();

            ViewBag.StartDate = start.ToString("yyyy-MM");
            ViewBag.EndDate = end.ToString("yyyy-MM");

            return View(report);
        }

        [Route("Customers")]
        public IActionResult Customers(string startDate, string endDate)
        {
            DateOnly start = string.IsNullOrEmpty(startDate) ? DateOnly.FromDateTime(new DateTime(2024, 1, 1)) : DateOnly.ParseExact(startDate, "yyyy-MM", CultureInfo.InvariantCulture);
            int year, month;
            DateOnly end = string.IsNullOrEmpty(endDate) ? DateOnly.FromDateTime(DateTime.Now) : new DateOnly(
                                                                                                        year = DateOnly.ParseExact(endDate, "yyyy-MM", CultureInfo.InvariantCulture).Year,
                                                                                                        month = DateOnly.ParseExact(endDate, "yyyy-MM", CultureInfo.InvariantCulture).Month,
                                                                                                        DateTime.DaysInMonth(year, month));
            var report = (from invoice in _context.SalesInvoices
                          where invoice.Date >= start && invoice.Date <= end
                          group invoice by new
                          {
                              Year = invoice.Date.Value.Year,
                              Month = invoice.Date.Value.Month,
                              CustomerId = invoice.CustomerId,
                              CustomerName = invoice.Customer.FirstName + " " + invoice.Customer.LastName + " (" + invoice.Customer.Username +")"
                          } into groupedData
                          select new CustomersVM
                          {
                              Month = groupedData.Key.Month.ToString("00") + "/" + groupedData.Key.Year,
                              Id = groupedData.Key.CustomerId,
                              Name = groupedData.Key.CustomerName,
                              Quantity = groupedData.Count(),
                              TotalPayment = groupedData.Sum(x => x.TotalPayment)
                          }).ToList();

            ViewBag.StartDate = start.ToString("yyyy-MM");
            ViewBag.EndDate = end.ToString("yyyy-MM");

            return View(report);
        }

        [Route("Invoices")]
        public IActionResult Invoices(string startDate, string endDate)
        {
            DateOnly start = string.IsNullOrEmpty(startDate) ? DateOnly.FromDateTime(new DateTime(2024, 1, 1)) : DateOnly.ParseExact(startDate, "yyyy-MM", CultureInfo.InvariantCulture);
            int year, month;
            DateOnly end = string.IsNullOrEmpty(endDate) ? DateOnly.FromDateTime(DateTime.Now) : new DateOnly(
                                                                                                        year = DateOnly.ParseExact(endDate, "yyyy-MM", CultureInfo.InvariantCulture).Year,
                                                                                                        month = DateOnly.ParseExact(endDate, "yyyy-MM", CultureInfo.InvariantCulture).Month,
                                                                                                        DateTime.DaysInMonth(year, month));
            var report = (from invoice in _context.SalesInvoices
                          where invoice.Date >= start && invoice.Date <= end
                          group invoice by new
                          {
                              Year = invoice.Date.Value.Year,
                              Month = invoice.Date.Value.Month,
                          } into groupedData
                          select new InvoicesVM
                          {
                              Month = groupedData.Key.Month.ToString("00") + "/" + groupedData.Key.Year,
                              Quantity = groupedData.Count(),
                              Total = groupedData.Sum(x => x.TotalPayment)
                          }).ToList();

            ViewBag.StartDate = start.ToString("yyyy-MM");
            ViewBag.EndDate = end.ToString("yyyy-MM");

            return View(report);
        }



    }
}
