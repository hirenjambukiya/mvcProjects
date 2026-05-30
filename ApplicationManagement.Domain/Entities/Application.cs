using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMS.Domain.Entities
{
    public class Applications:BaseEntity
    {
        public int UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Age { get; set; }
        public string Gender { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string District { get; set; } = string.Empty;

        public string Pincode { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;

        public string Status { get; set; } = "Pending";

        public User User { get; set; } = default!;

        public ICollection<ApplicationDocument> Documents { get; set; } = new List<ApplicationDocument>();
        public ICollection<ApplicationReview> Reviews { get; set; } = new List<ApplicationReview>();
    }
}
