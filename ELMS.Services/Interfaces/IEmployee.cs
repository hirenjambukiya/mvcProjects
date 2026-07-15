using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ELMS.Models.DTOs;

namespace ELMS.Services.Interfaces
{
    public interface IEmployee
    {
        LeaveSummaryDto GetLeaveSummaryByEmployeeId(Int64 UserId);
    }
}
