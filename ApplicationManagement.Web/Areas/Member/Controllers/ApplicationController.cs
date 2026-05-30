using AMS.Application.Constants;
using AMS.Application.DTOs;
using AMS.Application.Interfaces;
using AMS.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AMS.Web.Areas.Member.Controllers
{
    [Authorize]
    [Area("Member")]
    public class ApplicationController : Controller
    {
        private readonly ILogger<ApplicationController> _logger;
        private readonly IApplicationService _applicationService;
        public ApplicationController(IApplicationService applicationService, ILogger<ApplicationController> logger)
        {
            _applicationService = applicationService;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] ApplicationCreateDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(dto);
                }

                int userId = Convert.ToInt32(User.FindFirstValue("UserId"));

                await _applicationService.CreateAsync(dto, userId);

                TempData["Success"] = ErrorMsgs.ApplicationSubmittedSuccessfully;

                return RedirectToAction(nameof(MyApplications));
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<IActionResult> MyApplications()
        {
            try
            {
                int userId = Convert.ToInt32(
                        User.FindFirstValue("UserId"));

                var applications =
                    await _applicationService.GetByUserIdAsync(userId);

                return View(applications);
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
