using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using System.Threading.Tasks;
using DBPROJ_V2.Models;
using Microsoft.Extensions.Logging;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace DBPROJ_V2.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly IEmailSender _emailSender;
        private readonly ILogger<AccountController> _logger;
        private readonly ApplicationDbContext _context;
        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, IConfiguration configuration, IEmailSender emailSender, ILogger<AccountController> logger, ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _emailSender = emailSender;
            _logger = logger;
            _context = context;
        }

        [HttpGet]
        public IActionResult ExternalLogin(string provider, string returnUrl = null)
        {
            var redirectUrl = Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return Challenge(properties, provider);
        }
        [HttpGet]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
        {
            returnUrl ??= Url.Content("~/");
            if (remoteError != null)
            {
                ModelState.AddModelError(string.Empty, $"Error from external provider: {remoteError}");
                return View("Login");
            }

            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                _logger.LogError("Error loading external login info.");
                return RedirectToAction("Error","Home");
            }

            // Log the external login information
            _logger.LogInformation("ExternalLoginInfo: LoginProvider={LoginProvider}, ProviderKey={ProviderKey}, DisplayName={DisplayName}",
                info.LoginProvider, info.ProviderKey, info.ProviderDisplayName);
            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
            if (result.Succeeded)
            {
                //check if account already exists
                var user = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);
                if (user != null)
                {
                    _logger.LogInformation("External login succeeded.");
                    return LocalRedirect(returnUrl);
                }
                else
                {
                    var email = info.Principal.FindFirstValue(ClaimTypes.Email);
                    return RedirectToAction("Details","Account",email);
                }
            }
            else if (result.RequiresTwoFactor)
            {
                _logger.LogInformation("External login requires two-factor authentication.");
                return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = false });
            }
            else if (result.IsLockedOut)
            {
                _logger.LogWarning("User is locked out.");
                return RedirectToPage("./Lockout");
            }
            else
            {
                // Log the not allowed reason
                if (result.IsNotAllowed)
                {
                    _logger.LogWarning("External login not allowed. The user might need to confirm their email.");
                }

                _logger.LogWarning("External login failed. Attempting to create a new user.");

                var email = info.Principal.FindFirstValue(ClaimTypes.Email);
                if (email == null)
                {
                    _logger.LogError("Email claim not received from external provider.");
                    return View("Error");
                }

                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    var locations = new List<string> { "United States", "Canada", "United Kingdom", "Australia", "Germany" }; // Add more as needed
                    ViewData["Locations"] = locations;
                    return RedirectToAction("Details", "Account", new { email = email });
                }
                else
                {
                    _logger.LogWarning("User already exists. Attempting to add external login.");
                    //check if user email confirmed
                    if (!user.EmailConfirmed)
                    {
                        _logger.LogWarning("User email not confirmed. Redirecting to error page.");
                        return RedirectToAction("RegisterConfirmation", "Account", new { email = user.Email });
                    }
                    var loginResult = await _userManager.AddLoginAsync(user, info);
                    if (loginResult.Succeeded)
                    {
                        _logger.LogInformation("External login added to existing user successfully.");
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
                    else
                    {
                        _logger.LogError("Failed to add external login to existing user: {Errors}", string.Join(", ", loginResult.Errors.Select(e => e.Description)));
                    }
                }
                return View("Error");
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }
        [HttpGet]
        public async Task<IActionResult> LoginForm(string username,string password, string userType)
        {
            if (password == null || username == null)
            {
                TempData["msg"] = "Please fill in all fields!";
                return RedirectToAction("Login","Home");
            }

            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
            {
                TempData["msg"] = "Login Failed!";
                return RedirectToAction("Login","Home");
            }
            if(user.EmailConfirmed == false)
            {
                TempData["msg"] = "Please confirm your email!";
                return RedirectToAction("Login","Home");
            }
            var result = await _signInManager.PasswordSignInAsync(user, password, false, false);
            if (!result.Succeeded)
            {
                TempData["msg"] = "Login Failed!";
                return RedirectToAction("Login","Home");
            }

            var uType = string.IsNullOrEmpty(userType) ? "" : userType.ToLower();
            switch (uType)
            {
                case "admin":
                    return RedirectToAction("Index", "Home");
                    //return RedirectToAction("Index", "Admin");
                case "mem":
                    return RedirectToAction("Index", "Home");
                    //return RedirectToAction("Index", "Member");
                default:
                    return RedirectToAction("Index", "Home");
            }
        }
        [HttpGet]
        public async Task<IActionResult> register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new IdentityUser { UserName = model.Email, Email = model.Email };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Scheme);

                    await _emailSender.SendEmailAsync(model.Email, "Confirm your email",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");
                    return RedirectToAction("RegisterConfirmation", "Account", new { email = user.Email });
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            // If we got this far, something failed, redisplay form
            return View(model);
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string email, string token)
        {
            if (email == null || token == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var user = _context.UserProfiles
                .Include(u => u.User) // Include the User navigation property
                .FirstOrDefault(u => u.User.Email == email && u.UserId == token);

            if (user == null)
            {
                return View("Error", "Home");
            }

            // Confirm the user's email
            user.User.EmailConfirmed = true;
            _context.SaveChanges();

            return View();
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }
        [HttpGet]
        public IActionResult Details(string email)
        {
            var locations = new List<string> { "United States", "Canada", "United Kingdom", "Australia", "Germany" }; // Add more as needed
            ViewData["Locations"] = locations;
            ViewData["Email"] = new string (email);  
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DetailsAdd([Bind] DetailsViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new IdentityUser { UserName = model.Email, Email = model.Email };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    // Save additional user info
                    var userProfile = new UserProfile
                    {
                        UserId = user.Id,
                        FullName = model.FullName,
                        Location = model.Location
                    };
                    _context.UserProfiles.Add(userProfile);
                    await _context.SaveChangesAsync();

                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var callbackUrl = Url.Action(nameof(ConfirmEmail), "Account", new { userId = user.Id, code }, protocol: HttpContext.Request.Scheme);

                    await _emailSender.SendEmailAsync(model.Email, "Confirm your email",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    return RedirectToAction("RegisterConfirmation", "Account",new {email = user.Email});
                }
                else
                {
                    AddErrors(result);
                    return RedirectToAction("Error", "Home");
                }
            }
            else
            {
                // Log ModelState errors
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                // You can log errors to the console, a file, or a logging service
                foreach (var error in errors)
                {
                    Console.WriteLine(error);
                }
            }
            return RedirectToAction("Error", "Home");
        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult RegisterConfirmation(string email)
        {
            ViewData["Email"] = email;
            return View((object)email); // Pass email as the model to the view
        }

        [HttpPost]
        public async Task<IActionResult> CheckConfirmationStatus([FromBody] string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return BadRequest("Email cannot be null or empty");
            }

            var user = await _userManager.FindByEmailAsync(email);
            if (user != null && user.EmailConfirmed)
            {
                return Json(true);
            }
            else
            {
                return Json(false);
            }
        }

    }
}
