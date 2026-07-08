using AMS.Application.DTOs;
using AMS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMS.Application.Interfaces
{
    public interface IAuthService
    {
        Task<User?> LoginAsync(LoginDto loginDto);

        Task<(bool Success, string Message)> RegisterAsync(RegisterDto dto);
    }
}
