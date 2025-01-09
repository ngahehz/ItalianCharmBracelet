using ItalianCharmBracelet.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace ItalianCharmBracelet.Helpers
{
    public class Util
    {
        public static string GenerateRandomKey(int length = 5)
        {
            //const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            //var random = new Random();
            //return new string(Enumerable.Repeat(chars, length)
            //  .Select(s => s[random.Next(s.Length)]).ToArray());
            var pattern = @"ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var sb = new StringBuilder();
            var rd = new Random();
            for (int i = 0; i < length; i++)
            {
                sb.Append(pattern[rd.Next(0, pattern.Length)]);
            }
            return sb.ToString();
        }

        public static string UploadImg(IFormFile Hinh, string folder)
        {
            try
            {
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Hinh", folder, Hinh.FileName);
                using (var myfile = new FileStream(path, FileMode.CreateNew))
                {
                    Hinh.CopyTo(myfile);
                }
                return Hinh.FileName;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        public static string GenerateID(ItalianCharmBraceletContext context, string prefix)
        {
            var maxID = 0;
            string temp = !string.IsNullOrEmpty(prefix) && prefix.Length >= 5
                                                        ? prefix.Substring(0, 5) : prefix;
            switch (temp)
            {
                case "KH":
                    maxID = context.Customers.Any() ? context.Customers
                                .AsEnumerable()
                                .Select(c => int.Parse(c.Id.Substring(2))) 
                                .Max() : 0;
                    return $"{prefix}{(maxID + 1).ToString().PadLeft(5, '0')}";

                case "HDB":
                    maxID = context.SalesInvoices.Any() ? context.SalesInvoices
                                .AsEnumerable()
                                .Select(c => int.Parse(c.Id.Substring(3)))
                                .Max() : 0;
                    return $"{prefix}{(maxID + 1).ToString().PadLeft(6, '0')}";

                case "HDN":
                    maxID = context.PurchaseInvoices.Any() ? context.PurchaseInvoices
                                .AsEnumerable()
                                .Select(c => int.Parse(c.Id))
                                .Max() : 99999;
                    return $"{maxID + 1}";

                case "VC":
                    maxID = context.Vouchers.Any() ? context.Vouchers
                                .AsEnumerable()
                                .Select(c => int.Parse(c.Id))
                                .Max() : 99;
                    return $"{maxID + 1}";

                case "CHARM":
                    maxID = context.Charms.Any() ? context.Charms
                                .AsEnumerable()
                                .Where(c => c.Id.StartsWith(prefix))
                                .Select(c => int.Parse(c.Id.Substring(8)))
                                .Max() : 0;
                    return $"{prefix}_{(maxID + 1).ToString().PadLeft(3, '0')}";

                case "COMBO":
                    maxID = context.Charms.Any() ? context.Charms
                                .AsEnumerable()
                                .Where(c => c.Id.StartsWith(temp))
                                .Select(c => int.Parse(c.Id.Substring(8)))
                                .Max() : 0;
                    return $"{prefix}_{(maxID + 1).ToString().PadLeft(3, '0')}";

                default:
                    return string.Empty;
            }
        }
    }
}
