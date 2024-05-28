using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using DBPROJ_V2.Models;
using Microsoft.AspNetCore.Identity;

namespace DBPROJ_V2.Controllers
{
    public class HomeController : Controller
    {
        public HomeController(ILogger<HomeController> logger)
        {
        }

        public IActionResult Index()
        {
            TempData["msg"] = "";

            return View();
        }

        public IActionResult About()
        {
            return View();
        }
        [HttpGet]
        public IActionResult ExternalLogin(string provider, string returnUrl = null)
        {
            return RedirectToAction("ExternalLogin", "Account", new { provider, returnUrl });
        }
        [HttpPost]
        public IActionResult LoginForm([Bind]Cred credentials, string userType)
        {
            return RedirectToAction("LoginForm", "Account", new { username = credentials.Username, password = credentials.Password, userType = userType });
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // LOGIN LOGIC USING FORM
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            var reqId = Activity.Current == null ? HttpContext.TraceIdentifier : Activity.Current.Id;
            return View(new ErrorViewModel { RequestId = reqId });
        }
        //Registration
        public IActionResult Register()
        {
            var locations = new List<string> { "United States", "Canada", "United Kingdom", "Australia", "Germany" }; // Add more as needed
            ViewData["Locations"] = locations;
            return View();
        }
        public IActionResult regLogic([Bind]RegisterViewModel registerViewModel)
        {
            if (ModelState.IsValid)
            {
                return RedirectToAction("register","Account",registerViewModel);
            }
            else
            {
                TempData["msg"] = "Please fill in all fields!";
                return RedirectToAction("Register");
            }
        }
    }
}
