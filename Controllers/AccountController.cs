using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using SampleSecureWeb.Data;
using SampleSecureWeb.Models;
using SampleSecureWeb.ViewModel;


namespace SampleSecureWeb.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUser _userData;

        public AccountController(IUser user)
        {
            _userData = user;
        }
        // GET: AccountController
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Register()
        {
            return View();
        }

        private bool IsPasswordComplex(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                return false;
            // Ensure password is at least 12 characters long and contains at least one uppercase letter, one lowercase letter, and one number.
            if (password.Length < 12)
                return false;
            if (!password.Any(char.IsUpper))
                return false;
            if (!password.Any(char.IsLower))
                return false;
            if (!password.Any(char.IsDigit))
                return false;
            return true;
        }
        [HttpPost]
        public ActionResult Register(RegistrationViewModel registrationViewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (!IsPasswordComplex(registrationViewModel.Password))
                    {
                        ViewBag.PasswordError = "Password must be at least 12 characters long and contain at least one uppercase letter, one lowercase letter, and one number.";
                        return View(registrationViewModel);
                    }
                    var user = new Models.User
                    {
                        Username = registrationViewModel.Username,
                        Password = registrationViewModel.Password,
                        RoleName = "Contributor"
                    };
                    _userData.Registration(user);
                    return RedirectToAction("Index", "Home");
                }
            }
            catch (System.Exception ex)
            {
                ViewBag.Message = ex.Message;
            }
            return View(registrationViewModel);
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Login(LoginViewModel loginViewModel)
        {
            try
            {

                var user = new User
                {
                    Username = loginViewModel.Username,
                    Password = loginViewModel.Password
                };

                var LoginUser = _userData.Login(user);
                if (LoginUser == null)
                {
                    ViewBag.Error = "Invalid login attempt";
                    return View(loginViewModel);
                }

                var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, user.Username)
                    };
                var identity = new ClaimsIdentity(claims,
                    CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    principal,
                    new AuthenticationProperties
                    {
                        IsPersistent = loginViewModel.RememberLogin
                    });
                return RedirectToAction("Index", "Home");
            }
            catch (System.Exception ex)
            {
                ViewBag.Message = ex.Message;
            }
            return View(loginViewModel);
        }
    }
}
