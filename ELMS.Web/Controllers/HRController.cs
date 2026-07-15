using ELMS.Commons.Constants;
using ELMS.Models.Commons;
using ELMS.Models.DTOs;
using ELMS.Models.Models;
using ELMS.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ELMS.Web.Controllers
{
    public class HRController : Controller
    {
        private readonly IHR _hr;
        public HRController(IHR hR)
        {
            _hr = hR;
        }
        public IActionResult Dashboard()
        {
            LeaveSummaryDto leaveSummaryDto = _hr.GetLeaveSummary();
            return View(leaveSummaryDto);
        }
        [HttpPost]
        public IActionResult GetEmployeesLeaveList([FromBody] DataTableRequest request)
        {
            int pageNumber = (request.Start / request.Length) + 1;
            var result = _hr.GetEmployeesLeaveList(
                    request.Search?.Value,
                    pageNumber,
                    request.Length);
            result.Draw = request.Draw;

            return Ok(result);
        }

        [HttpPost]
        public IActionResult UpdateLeaveStatus([FromBody]UpdateLeaveStatus updateLeaveStatus)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json( new
                    {
                        success = false,
                        message = "Invalid Model"
                    });
                }
                switch (updateLeaveStatus.ActionType)
                {
                    case Commoncnt.Approved:
                        _hr.ApproveleavebyId(updateLeaveStatus.LeaveId);
                        return Json(new
                        {
                            success = true,
                            message = "Leave approved successfully."
                        });
                    case Commoncnt.Rejected:
                        _hr.RejectleavebyId(updateLeaveStatus.LeaveId, updateLeaveStatus.HRComment);
                        return Json(new
                        {
                            success = true,
                            message = "Leave rejected successfully."
                        });
                    default:
                        return Json(new
                        {
                            success = false,
                            message = "Invalid action type."
                        });
                }
            }
            catch (Exception)
            {
                var result = new
                {
                    success = false,
                    message = "Internal server error occurs."
                };
                return Json(result);
            }
        }
    }
}
