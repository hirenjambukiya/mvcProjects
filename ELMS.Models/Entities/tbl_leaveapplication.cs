using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ELMS.Commons.Enums;

namespace ELMS.Models.Entities
{
    public class tbl_leaveapplication: baseEntity
    {
        public Int64? LeaveId { get; set; }
        public Int64 UserId { get; set; }
        public LeaveStatus LeaveStatusId { get; set; }
        public LeaveType LeaveType { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public  required string Reason { get; set; }
        public string? HRComment { get; set; }
        public string? AttachedFileName { get; set; }
        
    }
}
