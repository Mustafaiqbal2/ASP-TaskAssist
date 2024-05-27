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
        Database db = new Database();

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
            if(credentials.password == null || credentials.username == null)
            {
                TempData["msg"] = "Please fill in all fields!";
                return RedirectToAction("Login");
            }
            var uType = string.IsNullOrEmpty(userType) ? "" : userType.ToLower();
            switch(uType)
            {
                case "admin":
                    Console.WriteLine("Admin");
                    break;
                case "mem":
                    Console.WriteLine("Member");
                    break;
            }
            bool flag = db.validateLogin(credentials, uType);
            if (flag)
            {
                  switch  (uType)
                    {
                        case "admin":
                            return RedirectToAction("Index", "Admin", null);
                        case "mem":
                            return RedirectToAction("Member");
                        default: 
                            return RedirectToAction("About");
                    }
            }
            else
            {
                TempData["msg"] = "Login Failed!";
                return RedirectToAction("Login");
            }
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
            return View();
        }
        public IActionResult regLogic([Bind]RegisterViewModel registerViewModel)
        {
            /*
            if (ModelState.IsValid)
            {
                bool flag = db.registerUser(registerViewModel);
                if (flag)
                {
                    TempData["msg"] = "Registration Successful!";
                    return RedirectToAction("Login");
                }
                else
                {
                    TempData["msg"] = "Registration Failed!";
                    return RedirectToAction("Register");
                }
            }
            else
            {
                TempData["msg"] = "Please fill in all fields!";
                return RedirectToAction("Register");
            }
            */
            return RedirectToAction("Login");
        }
    }
}
