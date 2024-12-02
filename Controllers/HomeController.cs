using ItalianCharmBracelet.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace ItalianCharmBracelet.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult NotFound404()
        {
            var greetings = new List<string>
            {
                "Oops! 😅 Looks like you took a wrong turn, but no worries – let’s get you back on track!",
                "This page is playing hide and seek! 🤗 Let’s help you find what you need.",
                "404 error – but you’re not lost to us! 🧭 Let’s find your way home.",
                "Well, this is awkward… 😳 Looks like the page went on vacation. Let’s find another path!",
                "Uh-oh! 🚧 It seems we misplaced this page. Can we show you something else?",
                "Don’t worry, even the best of us get lost sometimes! 🐾 Let’s explore together!",
                "Page not found! 🚀 Let’s take off and discover something new!"
            };

            var random = new Random();
            var randomGreeting = greetings[random.Next(greetings.Count)];

            ViewData["RandomGreeting404"] = randomGreeting;
            return View();
        }

        public IActionResult RequireLogin(string returnUrl)
        {
            var greetings = new List<string>
            {
                "Oops! 🚪🔒 You need to log in to open this door. Let’s get you signed in!",
                "Hey there! 👋 To continue your journey, please log in 🗝️✨.",
                "Uh-oh! 🛑 You need to be logged in to do this. Tap the login button and let’s go! 🚀",
                "🔑 Login required! Don’t worry, it’ll only take a sec. 😉",
                "Hi, friend! 🐻 This feature is for logged-in users only. Sign in to join the fun! 🎉",
                "Looks like you’re not signed in yet! 😅 Click login and let’s fix that! ✅",
                "Hold on! 🕒 You’ll need to log in to unlock this treasure 🏆✨.",
                "👋 Hi there! Logging in gives you full access to this feature. Go ahead and tap login! 🌟"
            };

            var random = new Random();
            var randomGreeting = greetings[random.Next(greetings.Count)];

            ViewData["RandomGreetingLogin"] = randomGreeting;
            return View("RequireLogin", returnUrl);
        }

        public IActionResult Contact()
        {
            var greetings = new List<string>
            {
                "We're all ears! Drop us a message and let’s make some magic happen!",
                "Have a question or just want to say hi? We’d love to hear from you!",
                "Let’s stay in touch! We’re here to help you with anything you need.",
                "You’ve got questions, we’ve got answers! Let’s chat!",
                "Reach out and give us a shout! We’re here to make your day better.",
                "Your message is like a little treasure to us – can't wait to read it!"
            };

            var random = new Random();
            var randomGreeting = greetings[random.Next(greetings.Count)];

            ViewData["RandomGreetingContact"] = randomGreeting;
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}
