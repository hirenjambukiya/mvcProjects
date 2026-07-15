using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ELMS.Commons.Enums;

namespace ELMS.Models.Entities
{
    public class mst_leavestatus: baseEntity
    {
        public Int64 LeaveStatusId { get; set; }
        public LeaveStatus LeaveType { get; set; }
        public string? Description { get; set; }
    }
}
