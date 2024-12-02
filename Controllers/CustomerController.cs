using AutoMapper;
using ItalianCharmBracelet.Data;
using ItalianCharmBracelet.Helpers;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ItalianCharmBracelet.ViewModels;

namespace ItalianCharmBracelet.Controllers
{
    public class CustomerController : Controller
    {
        private readonly ItalianCharmBraceletContext _context;
        private readonly IMapper _mapper;

        public CustomerController(ItalianCharmBraceletContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            return View();
        }

        #region Register
        [HttpGet]
        public IActionResult Register()
        {
            return PartialView();
        }

        [HttpPost]
        public IActionResult Register(RegisterVM model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var khachHang = _mapper.Map<Customer>(model);
                    khachHang.Id = Util.GenerateID(_context, "KH");
                    khachHang.RandomKey = Util.GenerateRandomKey();
                    khachHang.Password = model.Password.ToMd5Hash(khachHang.RandomKey);
                    khachHang.Role = "0";
                    khachHang.State = "True";//sẽ xử lý khi dùng Mail để active //SOS SOS SOS

                    _context.Add(khachHang);
                    _context.SaveChanges();
                    return Json(new { success = true, message = "Registration successful. Please check your email for activation." });

                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = ex.Message + "exc" });
                }
            }
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            //return Json(new { success = false, message = errors+"dd" });
            return PartialView("Register", model);
        }
        #endregion

        #region Login
        [HttpGet]
        public IActionResult Login()
        {
            return PartialView();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginVM model)
        {
            if (ModelState.IsValid)
            {
                var khachHang = _context.Customers.SingleOrDefault(x => x.Username == model.Username);
                if (khachHang != null)
                {
                    if (khachHang.State == "False")
                    {
                        ModelState.AddModelError("", "Tài khoản đã bị khóa, vui lòng liên hệ admin");
                    }
                    else
                    {
                        if (khachHang.Password == model.Password.ToMd5Hash(khachHang.RandomKey))
                        {
                            var claims = new List<Claim>
                            {
                                new Claim(ClaimTypes.Email, khachHang.Email),
                                new Claim(ClaimTypes.Role, "Customer"),
                                new Claim(MySetting.CLAIM_CUSTOMER, khachHang.Id),
                                new Claim(ClaimTypes.Name, khachHang.Username)
                            };
                            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);

                            return Json(new { success = true });
                        }
                        ModelState.AddModelError("", "Username or password is incorrect1");
                    }
                }
                // không có tài khoản
                ModelState.AddModelError("", "Username or password is incorrect2");
            }
            return PartialView("Login", model);
        }
        #endregion

        [Authorize]
        public IActionResult Profile()
        {
            var greetings = new List<string>
            {
                "Welcome back, superstar! 🌟 Ready to make today amazing?",
                "Hey there! 👋 Look who’s here – your fabulous self!",
                "Hello, you! 😎 Let’s dive into your profile and make magic happen!",
                "It’s your space, your story! 📖 Let’s see what makes you shine.",
                "Welcome to your personal haven 🏡 – where everything is all about you!",
                "You’ve got style, and your profile proves it! 💃 Dive in and explore!"
            };

            var random = new Random();
            var randomGreeting = greetings[random.Next(greetings.Count)];

            // Truyền câu ngẫu nhiên vào View thông qua ViewData
            ViewData["RandomGreetingProfile"] = randomGreeting;
            return View();
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
