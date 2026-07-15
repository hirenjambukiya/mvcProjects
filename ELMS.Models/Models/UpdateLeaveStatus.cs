using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELMS.Models.Models
{
    public class UpdateLeaveStatus
    {
        public Int64 LeaveId { get; set; }
        public required string ActionType { get; set; }
        public string? HRComment{get; set;}
    }
}
