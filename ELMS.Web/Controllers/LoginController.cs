using System.Security.AccessControl;
using System.Security.Claims;
using System.Threading.Tasks;
using ELMS.Commons.Constants;
using ELMS.Commons.Enums;
using ELMS.Helpers;
using ELMS.Models.DTOs;
using ELMS.Models.Entities;
using ELMS.Services.Interfaces;
using ELMS.Web.Helpers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ELMS.Web.Controllers
{
    public class LoginController : Controller
    {
        private readonly ILoging _loging;
        public LoginController(ILoging loging)
        {
            _loging = loging;
        }
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            if (!LoginValidationHelper.IsValidUserName(loginDto.UserName))
            {
                ModelState.AddModelError(nameof(loginDto.UserName), "Please enter a valid username.");
                return View(loginDto);

            }

            var user = _loging.GetUserByUserName(loginDto.UserName);

            if (user == null)
            {
                ModelState.AddModelError(nameof(loginDto.UserName), "Username does not exist.");
                return View(loginDto);

            }
            if (user.Password != loginDto.Password)
            {
                ModelState.AddModelError(nameof(loginDto.Password), "Incorrect password.");
                return View(loginDto);

            }
            var claims = new List<Claim> {
                    new Claim(ClaimTypes.Name, loginDto.UserName),
                    new Claim(ClaimTypes.NameIdentifier,user.UserId.ToString()),
                    new Claim(ClaimTypes.Role,((Roles)user.RoleId).ToString())
            };

            var indentity = new ClaimsIdentity(claims,CookieAuthenticationDefaults.AuthenticationScheme);
            var principle = new ClaimsPrincipal(indentity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,principle);

            SessionManager.Set(HttpContext, Sessioncnt.UserId, user.UserId);
            SessionManager.Set(HttpContext, Sessioncnt.UserName, loginDto.UserName);
            SessionManager.Set(HttpContext, Sessioncnt.Role, user.RoleId);

            switch (user.RoleId)
            {
                case Roles.Admin:
                    return RedirectToAction("Dashboard", "Admin");
                case Roles.Employee:
                    return RedirectToAction("Dashboard", "Employee");
                case Roles.HR:
                    return RedirectToAction("Dashboard", "HR");
                default:
                    return RedirectToAction("Login", "Login");
            }
            
        }
        public IActionResult Register()
        {
            var model = new RegisterDto();

            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(RegisterDto registorDto)
        {

            if (!ModelState.IsValid)
            {
                return View(registorDto);
            }
            _loging.InsertUser(new mst_users
            {
                UserId = SessionManager.Get<Int64>(HttpContext,Sessioncnt.UserId),
                FirtsName = registorDto.FirstName,
                LastName = registorDto.LastName,
                Password = registorDto.Password,
                EmailAddress = registorDto.EmailAddress,
                RoleId = registorDto.Role.HasValue ? registorDto.Role.Value : Roles.Employee,
            });
            return RedirectToAction("Login", "Login");
        }
        public async Task<IActionResult> LogOut()
        {
            await HttpContext.SignOutAsync();
            SessionManager.Clear(HttpContext);

            return RedirectToAction("Login","Login");
        }

        [Authorize]
        public IActionResult UpdateProfile()
        {
            var user = _loging.GetUserByUserName(SessionManager.Get<string>(HttpContext,Sessioncnt.UserName));
            var userdata = new UpdateProfileDto
            {
                FirstName = user.FirtsName,
                LastName = user.LastName,
                EmailAddress = user.EmailAddress,
                Password = user.Password,
            };
            return View(userdata);
        }

        [Authorize]
        [HttpPost]
        public IActionResult UpdateProfile(UpdateProfileDto updateProfileDto)
        {
            if (!ModelState.IsValid)
            {
                return View(updateProfileDto);
            }
            _loging.InsertUser(new mst_users
            {
                UserId = SessionManager.Get<Int64>(HttpContext, Sessioncnt.UserId),
                FirtsName = updateProfileDto.FirstName,
                LastName = updateProfileDto.LastName,
                Password = updateProfileDto.Password,
                EmailAddress = updateProfileDto.EmailAddress,
            });

            return RedirectToAction("Dashboard", "Employee");
        }
    }
}
