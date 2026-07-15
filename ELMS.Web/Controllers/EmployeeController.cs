using ELMS.Commons.Constants;
using ELMS.Models.DTOs;
using ELMS.Services.Interfaces;
using ELMS.Web.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ELMS.Web.Controllers
{
    [Authorize]
    public class EmployeeController : Controller
    {
        private readonly IEmployee _employeeService;
        public EmployeeController(IEmployee employee)
        {
            _employeeService = employee;
        }
        public IActionResult Dashboard()
        {
            LeaveSummaryDto leaveSummaryDto = new LeaveSummaryDto();
            leaveSummaryDto = _employeeService.GetLeaveSummaryByEmployeeId(SessionManager.Get<Int64>(HttpContext,Sessioncnt.UserId));
            return View(leaveSummaryDto);
        }
    }
}
