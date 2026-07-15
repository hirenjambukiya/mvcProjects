using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELMS.Models.DTOs
{
    public class LeaveSummaryDto
    {
        public int TotalLeave { get; set; } = 0;

        public int PendingLeave { get; set; } = 0;

        public int ApprovedLeave { get; set; } = 0;

        public int RejectedLeave { get; set; } = 0;

    }
}
