using AMS.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMS.Application.Interfaces
{
    public interface ILogService
    {
        Task LogAsync(LogEntryDto log);
    }
}
