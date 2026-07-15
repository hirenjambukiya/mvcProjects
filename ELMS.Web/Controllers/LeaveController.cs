using System.IO;
using System.Reflection;
using ELMS.Commons.Constants;
using ELMS.Commons.Enums;
using ELMS.Models.Commons;
using ELMS.Models.DTOs;
using ELMS.Models.Entities;
using ELMS.Services.Interfaces;
using ELMS.Web.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ELMS.Web.Controllers
{
    [Authorize]
    public class LeaveController : Controller
    {
        private readonly ILeave _leave;
        public LeaveController(ILeave leave)
        {
            _leave = leave;
        }
        public IActionResult LeaveHistory()
        {
            LeaveForm leaveForm = new LeaveForm {};
            return View(leaveForm);
        }

        [HttpGet]
        public IActionResult GetEmptyLeaveForm()
        {
            return PartialView("_LeaveForm", new LeaveForm());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult LeaveHistory([FromForm] LeaveForm leaveForm)
        {
            try
            {
                string? safeFileName = string.Empty;
                if (!ModelState.IsValid)
                {
                    return PartialView("_LeaveForm", leaveForm);

                }
                if (leaveForm.EndDate < leaveForm.StartDate)
                {
                    ModelState.AddModelError(nameof(leaveForm.EndDate),
                        "End Date must be greater than or equal to Start Date.");
                    return PartialView("_LeaveForm", leaveForm);
                }
                TimeSpan timeSpan = (TimeSpan)(leaveForm.EndDate - leaveForm.StartDate);

                if (timeSpan.Days>30)
                {
                    ModelState.AddModelError(nameof(leaveForm.EndDate),
                        $"Not allow morthen 30 days leave, your apply leave is {timeSpan.Days} day.");
                    return PartialView("_LeaveForm", leaveForm);
                }

                if (leaveForm.Attchement != null)
                {
                    string rootpath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Attachments");
                    if (!Directory.Exists(rootpath))
                    {
                        Directory.CreateDirectory(rootpath);
                    }
                    FileInfo fileInfo = new FileInfo(leaveForm.Attchement.FileName);
                    string extention = Path.GetExtension(leaveForm.Attchement.FileName);
                    safeFileName = $"{SessionManager.Get<Int64>(HttpContext, Sessioncnt.UserId)}_{Guid.NewGuid()}{extention}";
                    string filePath = Path.Combine(rootpath, safeFileName);
                    using (var stream = new FileStream(filePath,FileMode.Create))
                    {
                         leaveForm.Attchement.CopyToAsync(stream);
                         
                    }

                }
                var leaveApplication = new tbl_leaveapplication
                {
                    LeaveId = leaveForm.LeaveId,
                    UserId = SessionManager.Get<Int64>(HttpContext, Sessioncnt.UserId),
                    LeaveType = (LeaveType)leaveForm.LeaveType,
                    StartDate = (DateTime)leaveForm.StartDate,
                    EndDate = (DateTime)leaveForm.EndDate,
                    LeaveStatusId = LeaveStatus.Pending,
                    Reason = leaveForm.Reason,
                    CreateAt = DateTime.Now,
                    AttachedFileName = safeFileName
                };
                _leave.ApplyLeave(leaveApplication);
                return Json(new
                {
                    success = true,
                    message = "Leave applied successfully."
                });
            }
            catch (Exception)
            {

                return Json(new
                {
                    success = false,
                    message = "Internal Server Error."
                });
            }

        }

        [HttpPost]
        public IActionResult GetLeaveList([FromBody]DataTableRequest request)
        {
            try
            {
                long userId = SessionManager.Get<long>(HttpContext, Sessioncnt.UserId);

                int pageNumber = (request.Start / request.Length) + 1;

                var result = _leave.GetLeavebyUserid(
                    userId,
                    request.Search?.Value,
                    pageNumber,
                    request.Length);
                result.Draw = request.Draw;

                return Ok(result);

            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpGet]
        public IActionResult GetLeave(long leaveId)
        {
            var leave = _leave.GetLeaveById(leaveId);

            return Json(leave);

        }
    }
}
