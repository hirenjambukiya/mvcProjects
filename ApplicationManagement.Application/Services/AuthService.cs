using AMS.Application.Constants;
using AMS.Application.DTOs;
using AMS.Application.Interfaces;
using AMS.Application.Security;
using AMS.Domain.Entities;
using AMS.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AMS.Domain.Enums.Enums;

namespace AMS.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        public AuthService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public Task<User?> LoginAsync(LoginDto loginDto)
        {
            try
            {
                var user = _userRepository.GetUserByEmailAsync(loginDto.Email).Result;

                if (user == null)
                    return Task.FromResult<User?>(null);

                string passwordHash = PasswordHelper.HashPassword(loginDto.Password);

                if (user.PasswordHash != passwordHash)
                    return Task.FromResult<User?>(null);

                return Task.FromResult<User?>(user);
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<(bool Success, string Message)> RegisterAsync(RegisterDto dto)
        {
            try
            {
                var existingUser = await _userRepository.GetUserByEmailAsync(dto.Email);

                if (existingUser != null)
                {
                    return (false, ErrorMsgs.EmailAlreadyExists);
                }

                User user = new()
                {
                    Name = dto.Name,
                    Email = dto.Email,
                    PasswordHash = PasswordHelper.HashPassword(dto.Password),
                    Role = UserRole.Member
                };

                await _userRepository.AddUserAsync(user);

                await _userRepository.SaveChangesAsync();

                return (true, ErrorMsgs.RegistrationSuccessful);
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
