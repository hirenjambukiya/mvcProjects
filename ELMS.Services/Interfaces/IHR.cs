using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ELMS.Models.DTOs;

namespace ELMS.Services.Interfaces
{
    public interface IHR
    {
        LeaveSummaryDto GetLeaveSummary();
        DataTableResponse<LeaveListDto> GetEmployeesLeaveList(string? Search = null, int PageNumber = 1, int PageSize = 10);
        void ApproveleavebyId(Int64 LeaveId);
        void RejectleavebyId(Int64 LeaveId, string HRComment);
    }
}
