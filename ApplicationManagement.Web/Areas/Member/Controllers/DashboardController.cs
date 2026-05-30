using AMS.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AMS.Web.Areas.Member.Controllers
{
    [Authorize]
    [Area("Member")]
    public class DashboardController : Controller
    {
        private readonly IApplicationService _service;

        public DashboardController(IApplicationService service)
        {
            _service = service;
        }
        public async Task<IActionResult> Index()
        {
            try
            {
                int userId = Convert.ToInt32(
                        User.FindFirstValue("UserId"));

                var memberDashboard = await _service.GetMemberDashboardDataAsync(userId);

                return View(memberDashboard);
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
