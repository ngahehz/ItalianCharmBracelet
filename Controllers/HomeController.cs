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
