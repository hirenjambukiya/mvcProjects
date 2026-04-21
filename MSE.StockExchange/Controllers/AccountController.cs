using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using MSE.StockExchange.Models.ViewModels;
using MSE.StockExchange.Services;

namespace MSE.StockExchange.Controllers;

public class AccountController : Controller
{
    private readonly IAuthService _authService;

    public AccountController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpGet]
    public IActionResult Login(string returnUrl = "/")
    {
        // If already logged in, redirect
        if (User.Identity != null && User.Identity.IsAuthenticated)
        {
            return LocalRedirect(returnUrl);
        }

        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = "/")
    {
        ViewData["ReturnUrl"] = returnUrl;

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        // Use the client-side encrypted password for authentication check
        var (result, user) = await _authService.AuthenticateAsync(model.Username, model.ClientEncryptedPassword);

        switch (result)
        {
            case LoginResult.Success:
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user!.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.RoleName)
                };

                var claimsIdentity = new ClaimsIdentity(
                    claims, CookieAuthenticationDefaults.AuthenticationScheme);

                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = false // Optional: could bind to a "Remember Me" checkbox
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                return LocalRedirect(returnUrl);

            case LoginResult.LockedOut:
                ModelState.AddModelError(string.Empty, "Account locked out. Please try again after 15 minutes.");
                break;

            case LoginResult.InvalidCredentials:
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                break;

            case LoginResult.NotActive:
                ModelState.AddModelError(string.Empty, "Account is disabled. Please contact support.");
                break;
        }

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }
    
    [HttpGet]
    public IActionResult AccessDenied()
    {
        return View();
    }

    [HttpGet]
    public IActionResult Register()
    {
        // If already logged in, redirect
        if (User.Identity != null && User.Identity.IsAuthenticated)
        {
            return LocalRedirect("/");
        }

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var (success, errorMessage) = await _authService.RegisterAsync(model.Username, model.Email, model.ClientEncryptedPassword, model.Role);

        if (success)
        {
            // Registration successful, redirect to login
            TempData["SuccessMessage"] = "Registration successful. Please login.";
            return RedirectToAction("Login");
        }

        ModelState.AddModelError(string.Empty, errorMessage);
        return View(model);
    }
}
