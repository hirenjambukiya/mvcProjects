using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ELMS.Models.DTOs;
using ELMS.Models.Entities;

namespace ELMS.Services.Interfaces
{
    public interface ILeave
    {
        void ApplyLeave(tbl_leaveapplication tbl_Leaveapplication);
        DataTableResponse<LeaveListDto> GetLeavebyUserid(Int64 UserId,string? Search = null, int PageNumber = 1,int PageSize = 10);
        LeaveForm GetLeaveById(long leaveId);
    }
}
