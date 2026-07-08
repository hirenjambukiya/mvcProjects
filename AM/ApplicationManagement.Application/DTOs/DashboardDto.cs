using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMS.Application.DTOs
{
    public class DashboardDto
    {
        public int TotalApplications { get; set; }

        public int PendingApplications { get; set; }

        public int ApprovedApplications { get; set; }

        public int RejectedApplications { get; set; }
    }
}
