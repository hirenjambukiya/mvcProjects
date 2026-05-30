using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace AMS.Domain.Entities
{
    public class ApplicationReview:BaseEntity
    {
        public int ApplicationId { get; set; }

        public int ReviewedByUserId { get; set; }

        public string Status { get; set; } = string.Empty;

        public string Remarks { get; set; } = string.Empty;

        public DateTime ReviewedDate { get; set; } = DateTime.Now;

        public Applications Applications { get; set; } = default!;
    }
}
