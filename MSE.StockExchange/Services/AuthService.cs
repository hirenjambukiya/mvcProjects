using System;
using System.Threading.Tasks;
using MSE.StockExchange.Repositories;
using MSE.StockExchange.Models.Domain;

namespace MSE.StockExchange.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;

    public AuthService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<(LoginResult Result, User? User)> AuthenticateAsync(string username, string clientEncryptedPassword)
    {
        var user = await _userRepository.GetUserByUsernameAsync(username);

        if (user == null)
        {
            return (LoginResult.InvalidCredentials, null);
        }

        if (!user.IsActive)
        {
            return (LoginResult.NotActive, null);
        }

        if (user.IsLockedOut && user.LockoutEnd.HasValue && user.LockoutEnd.Value > DateTime.UtcNow)
        {
            return (LoginResult.LockedOut, null);
        }

        // If lockout time has passed, reset it
        if (user.IsLockedOut && user.LockoutEnd.HasValue && user.LockoutEnd.Value <= DateTime.UtcNow)
        {
            await _userRepository.ResetFailedAttemptAsync(user.Id);
            user.IsLockedOut = false;
            user.FailedAttemptCount = 0;
            user.LockoutEnd = null;
        }

        // Verify Password (BCrypt)
        bool isPasswordValid = BCrypt.Net.BCrypt.Verify(clientEncryptedPassword, user.PasswordHash);

        if (!isPasswordValid)
        {
            user.FailedAttemptCount++;
            int maxFailedAttempts = 5;
            
            if (user.FailedAttemptCount >= maxFailedAttempts)
            {
                user.IsLockedOut = true;
                user.LockoutEnd = DateTime.UtcNow.AddMinutes(15);
            }

            await _userRepository.UpdateFailedAttemptAsync(user.Id, user.FailedAttemptCount, user.IsLockedOut, user.LockoutEnd);

            return (user.IsLockedOut ? LoginResult.LockedOut : LoginResult.InvalidCredentials, null);
        }

        // Reset failed attempts on success
        if (user.FailedAttemptCount > 0)
        {
            await _userRepository.ResetFailedAttemptAsync(user.Id);
        }

        return (LoginResult.Success, user);
    }

    public async Task<(bool Success, string ErrorMessage)> RegisterAsync(string username, string email, string clientEncryptedPassword, string roleName)
    {
        var existingUser = await _userRepository.GetUserByUsernameAsync(username);
        if (existingUser != null)
        {
            return (false, "Username already exists.");
        }

        // Hash the client encrypted password securely on server using BCrypt
        var serverHashedPassword = BCrypt.Net.BCrypt.HashPassword(clientEncryptedPassword);

        var newUser = new User
        {
            Username = username,
            Email = email,
            PasswordHash = serverHashedPassword,
            IsActive = true
        };

        try
        {
            await _userRepository.CreateUserAsync(newUser, roleName);
            return (true, string.Empty);
        }
        catch (Exception ex)
        {
            return (false, $"An error occurred during registration: {ex.Message}");
        }
    }

    public async Task<User?> GetUserByUsernameOrEmailAsync(string identifier)
    {
        return await _userRepository.GetUserByUsernameOrEmailAsync(identifier);
    }

    public async Task<(bool Success, string ErrorMessage)> ResetPasswordAsync(string identifier, string newClientEncryptedPassword)
    {
        var user = await _userRepository.GetUserByUsernameOrEmailAsync(identifier);
        
        if (user == null)
        {
            return (false, "User not found.");
        }

        try
        {
            var serverHashedPassword = BCrypt.Net.BCrypt.HashPassword(newClientEncryptedPassword);
            await _userRepository.UpdatePasswordAsync(user.Id, serverHashedPassword);
            
            // Optionally, reset failed attempts and unlock out
            if (user.IsLockedOut || user.FailedAttemptCount > 0)
            {
                await _userRepository.ResetFailedAttemptAsync(user.Id);
            }

            return (true, string.Empty);
        }
        catch (Exception ex)
        {
            return (false, $"An error occurred during password reset: {ex.Message}");
        }
    }
}
