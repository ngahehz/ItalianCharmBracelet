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
            return View();
        }

        [HttpPost]
        public IActionResult Register(RegisterVM model, IFormFile Hinh)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var khachHang = _mapper.Map<Customer>(model);
                    khachHang.RandomKey = Util.GenerateRandomKey();
                    khachHang.Password = model.Password.ToMd5Hash(khachHang.RandomKey);
                    khachHang.Role = "0";
                    khachHang.State = "True";//sẽ xử lý khi dùng Mail để active //SOS SOS SOS

                    if (Hinh != null)
                    {
                        khachHang.Img = Util.UploadImg(Hinh, "KhachHang");
                    }

                    _context.Add(khachHang);
                    _context.SaveChanges();
                    return RedirectToAction("Index", "Home");

                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }
            return View();
        }
        #endregion

        #region Login
        [HttpGet]
        public IActionResult Login(string? ReturnUrl)
        {
            ViewBag.ReturnUrl = ReturnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginVM model, string? ReturnUrl)
        {
            ViewBag.ReturnUrl = ReturnUrl;
            if (ModelState.IsValid)
            {
                var khachHang = _context.Customers.SingleOrDefault(x => x.Id == model.Username);
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
                                new Claim(ClaimTypes.Role, "Customer")
                            };
                            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);

                            if (Url.IsLocalUrl(ReturnUrl))
                            {
                                return Redirect(ReturnUrl);
                            }
                            return Redirect("/");
                        }
                        ModelState.AddModelError("", "Username or password is incorrect");
                    }
                }
                // không có tài khoản
                ModelState.AddModelError("", "Username or password is incorrect");
            }
            return View();
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
