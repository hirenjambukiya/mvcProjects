using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ELMS.Commons.Enums;

namespace ELMS.Models.DTOs
{
    public class LeaveListDto
    {
        public long LeaveId { get; set; }
        
        public string? UserName { get; set; }
        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int TotalDays { get; set; }

        public string Reason { get; set; }

        public LeaveType LeaveType { get; set; }
        public string LeaveTypeName => LeaveType.ToString();

        public LeaveStatus LeaveStatusId { get; set; }
        public string LeaveStatusName => LeaveStatusId.ToString();

        public string HRComment { get; set; }

        public string AttachedFileName { get; set; }

        public DateTime CreateAt { get; set; }

    }
}
