using AMS.Application.DTOs;
using AMS.Application.Interfaces;
using AMS.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AMS.Web.Areas.Admine.Controllers
{
    [Authorize]
    [Area("Admin")]

    public class InboxController : Controller
    {
        private readonly IApplicationService _service;
        private readonly ILogService _logService;
        public InboxController(IApplicationService service, ILogService logService)
        {
            _service = service;
            _logService = logService;
        }

        public async Task<IActionResult> Index()
        {
            var applications = await _service.GetAllAsync();

            return View(applications);
        }

        public async Task<IActionResult> Review(int id)
        {
            var application = await _service.GetByIdAsync(id);

            if (application == null)
                return NotFound();

            return View(application);
        }

        [HttpPost]
        public async Task<IActionResult> Review(ApplicationReviewDto dto)
        {
            int adminUserId = Convert.ToInt32(User.FindFirstValue("UserId"));

            await _service.ReviewAsync(dto, adminUserId);

            await _logService.LogAsync(new LogEntryDto
            {
                Level = "INFO",
                Message = $"Application #{dto.ApplicationId} reviewed with status {dto.Status}",
                UserEmail = User.Identity?.Name
            });
            TempData["Success"] = "Application reviewed successfully";

            return RedirectToAction(nameof(Index));
        }
    }
}
