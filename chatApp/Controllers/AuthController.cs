using System.Security.Claims;
using chatApp.Models;
using ChatApp.Common;
using ChatApp.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.Controllers
{
    public class AuthController : Controller
    {
        private readonly IUserService _userService;

        public AuthController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            var user = await _userService.GetUserByNameAsync(username);
            password = PasswordHelper.HashPassword(password); // In production: use a secure hash function
            if (user == null || user.PasswordHash != password) // In production: hash comparison
            {
                ModelState.AddModelError("", "Invalid username or password.");
                return View();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.Username)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            await _userService.UpdateOnlineStatusAsync(user.UserId, true);

            return RedirectToAction("Index", "Chat");
        }

        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(string username, string password, string displayName)
        {
            var existing = await _userService.GetUserByNameAsync(username);
            if (existing != null)
            {
                ModelState.AddModelError("", "Username already taken.");
                return View();
            }

            var newUser = new User
            {
                Username = username,
                DisplayName = displayName,
                PasswordHash = password, // In production: hash the password
                IsOnline = false,
                LastActive = DateTime.Now
            };

            var userId = await _userService.RegisterAsync(newUser);
            return RedirectToAction("Login");
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            await _userService.UpdateOnlineStatusAsync(userId, false);
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Login");
        }
    }
}
