using AMS.Application.Constants;
using AMS.Application.DTOs;
using AMS.Application.Interfaces;
using AMS.Application.Services;
using AMS.Domain.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static AMS.Domain.Enums.Enums;

namespace AMS.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAuthService _authService;
        private readonly ILogService _logService;
        public AccountController(ILogService logService, IAuthService authService)
        {
            _logService = logService;
            _authService = authService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([FromForm] LoginDto loginDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(loginDto);
                }
                var result = await _authService.LoginAsync(loginDto);
                
                
                if (result == null)
                {
                    await _logService.LogAsync(new LogEntryDto
                    {
                        Level = "WARNING",
                        Message = "Invalid login attempt",
                        UserEmail = loginDto.Email
                    });
                    ModelState.AddModelError(string.Empty, ErrorMsgs.InvalidUsernameOrPassword);
                    return View(loginDto);
                }

                await _logService.LogAsync(new LogEntryDto
                {
                    Level = "INFO",
                    Message = "User logged in successfully",
                    UserEmail = result.Email
                });

                #region Set cookie
                List<Claim> claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, result.Name),
                    new Claim(ClaimTypes.Role, result.Role.ToString()),
                    new Claim(ClaimTypes.Email, result.Email),
                    new Claim("UserId", result.Id.ToString())
                };

                ClaimsIdentity claimsIdentity = new(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                ClaimsPrincipal claimsPrincipal = new(claimsIdentity);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);
               
                return result.Role switch
                {
                    UserRole.Admin =>
                        RedirectToAction("Index", "Dashboard", new { area = "Admin" }),

                    UserRole.Member => RedirectToAction("Index","Dashboard", new { area = "Member" }),

                    _ => RedirectToAction("Login", "Account")
                };
                #endregion
            }
            catch (Exception)
            {
                return View(loginDto);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            try
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                TempData["Success"] = "Logged out successfully.";
                return RedirectToAction("Login", "Account");
            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return View(dto);

                var result = await _authService.RegisterAsync(dto);

                if (!result.Success)
                {
                    ModelState.AddModelError(string.Empty, result.Message);
                    return View(dto);
                }

                TempData["Success"] = result.Message;

                return RedirectToAction("Login","Account");
            }
            catch (Exception)
            {

                throw;
            }
        }
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
