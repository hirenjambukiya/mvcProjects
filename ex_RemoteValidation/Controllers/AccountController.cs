using Dapper;
using ex_RemoteValidation.Helper;
using ex_RemoteValidation.Models;
using Microsoft.AspNetCore.Mvc;

namespace ex_RemoteValidation.Controllers
{
    public class AccountController : Controller
    {
        private readonly DapperHelper _dapper;

        public AccountController(DapperHelper dapper)
        {
            _dapper = dapper;
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(userRegister model)
        {
            if (ModelState.IsValid)
            {
                // Save logic
                return RedirectToAction("Success");
            }
            return View(model);
        }

        [AcceptVerbs("Get", "Post")]
        public IActionResult IsUsernameAvailable(string username)
        {
            using var conn = _dapper.CreateConnection();
            var exists = conn.QueryFirstOrDefault<string>(
                "SELECT Username FROM Users_Remote WHERE Username = @Username",
                new { Username = username });

            return Json(exists == null ? true : $"Username '{username}' is already taken.");
        }

        [AcceptVerbs("Get", "Post")]
        public IActionResult IsEmailAvailable(string email)
        {
            using var conn = _dapper.CreateConnection();
            var exists = conn.QueryFirstOrDefault<string>(
                "SELECT Email FROM Users_Remote WHERE Email = @Email",
                new { Email = email });

            return Json(exists == null ? true : $"Email '{email}' is already registered.");
        }
    }

}
