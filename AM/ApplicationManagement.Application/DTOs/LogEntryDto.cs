using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMS.Application.DTOs
{
    public class LogEntryDto
    {
        public string Message { get; set; } = string.Empty;

        public string Level { get; set; } = string.Empty;

        public string? UserEmail { get; set; }

        public string? Exception { get; set; }
    }
}
