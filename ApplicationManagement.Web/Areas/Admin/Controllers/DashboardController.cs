using AMS.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AMS.Web.Areas.Admine.Controllers
{
    [Authorize]
    [Area("Admin")]
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
                var adminDashboard = await _service.GetDashboardDataAsync();

                return View(adminDashboard);
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
