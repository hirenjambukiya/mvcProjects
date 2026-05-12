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
    private readonly IOtpService _otpService;
    private readonly IEmailService _emailService;

    public AccountController(IAuthService authService, IOtpService otpService, IEmailService emailService)
    {
        _authService = authService;
        _otpService = otpService;
        _emailService = emailService;
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
                // Generate and send OTP instead of logging in immediately
                var otp = _otpService.GenerateOtp("Login", user!.Username);
                var emailSubject = "MSE Stock Exchange - Login OTP";
                var emailBody = $"<p>Your OTP for login is: <strong>{otp}</strong></p><p>This is valid for 5 minutes.</p>";
                await _emailService.SendEmailAsync(user.Email, emailSubject, emailBody);

                // Store username in TempData to pass to VerifyOtp GET
                TempData["LoginUsername"] = user.Username;

                return RedirectToAction("VerifyLoginOtp", new { returnUrl });

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

    [HttpGet]
    public IActionResult VerifyLoginOtp(string returnUrl = "/")
    {
        var username = TempData["LoginUsername"]?.ToString();
        if (string.IsNullOrEmpty(username))
        {
            return RedirectToAction("Login");
        }

        // Keep tempdata in case of refresh
        TempData.Keep("LoginUsername");

        var model = new VerifyLoginOtpViewModel { Username = username, ReturnUrl = returnUrl };
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> VerifyLoginOtp(VerifyLoginOtpViewModel model)
    {
        TempData.Keep("LoginUsername"); // Try to keep alive

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        bool isValid = _otpService.ValidateOtp("Login", model.Username, model.Otp);
        if (!isValid)
        {
            ModelState.AddModelError(string.Empty, "Invalid or expired OTP.");
            return View(model);
        }

        var user = await _authService.GetUserByUsernameOrEmailAsync(model.Username);
        if (user == null || !user.IsActive || user.IsLockedOut)
        {
             ModelState.AddModelError(string.Empty, "Account verification failed.");
             return View(model);
        }

        // Check lock out time
        if (user.IsLockedOut && user.LockoutEnd.HasValue && user.LockoutEnd.Value > System.DateTime.UtcNow)
        {
            ModelState.AddModelError(string.Empty, "Account locked out. Please try again later.");
            return View(model);
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.RoleName)
        };

        var claimsIdentity = new ClaimsIdentity(
            claims, CookieAuthenticationDefaults.AuthenticationScheme);

        var authProperties = new AuthenticationProperties
        {
            IsPersistent = false
        };

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity),
            authProperties);

        // Clear temp data
        TempData.Remove("LoginUsername");

        return LocalRedirect(model.ReturnUrl ?? "/");
    }

    [HttpGet]
    public IActionResult ForgotPassword()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var user = await _authService.GetUserByUsernameOrEmailAsync(model.Identifier);
        if (user != null && user.IsActive)
        {
            var otp = _otpService.GenerateOtp("ResetPassword", user.Username);
            var emailSubject = "MSE Stock Exchange - Password Reset";
            var emailBody = $"<p>Your OTP for password reset is: <strong>{otp}</strong></p><p>This is valid for 5 minutes.</p>";
            await _emailService.SendEmailAsync(user.Email, emailSubject, emailBody);
        }

        // Regardless of whether the user exists, show success to prevent user enumeration
        // We will pass the Username safely, or if not found, just pass the entered text
        return RedirectToAction("ResetPassword", new { username = user?.Username ?? model.Identifier });
    }

    [HttpGet]
    public IActionResult ResetPassword(string username)
    {
        var model = new ResetPasswordViewModel { Identifier = username };
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        bool isValid = _otpService.ValidateOtp("ResetPassword", model.Identifier, model.Otp);
        if (!isValid)
        {
            ModelState.AddModelError(string.Empty, "Invalid or expired OTP.");
            return View(model);
        }

        var (success, errorMsg) = await _authService.ResetPasswordAsync(model.Identifier, model.NewClientEncryptedPassword);
        
        if (!success)
        {
            ModelState.AddModelError(string.Empty, errorMsg);
            return View(model);
        }

        TempData["SuccessMessage"] = "Password has been reset successfully. Please login.";
        return RedirectToAction("Login");
    }
}
 