using EmailService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Pweb_2021.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Pweb_2021.Controllers
{
    public class HomeController : Controller
    {
        private readonly IEmailSender _emailSender;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, IEmailSender emailSender)
        {
            _emailSender = emailSender;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return RedirectToAction(nameof(Index), "Imoveis");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult Test()
        {
            var rng = new Random();
            var message = new Message(new string[] { "onikenx@pm.me" }, "Test email", "This is the content from our email.");
            try
            {
                _emailSender.SendEmail(message);

            }catch (Exception ex)
            {
                return View(ex.Message);
            }
            return View("Enviado com sucesso");
        }
    }
}
