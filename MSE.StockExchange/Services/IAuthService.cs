using System.Threading.Tasks;
using MSE.StockExchange.Models.Domain;

namespace MSE.StockExchange.Services;

public enum LoginResult
{
    Success,
    LockedOut,
    InvalidCredentials,
    NotActive
}

public interface IAuthService
{
    Task<(LoginResult Result, User? User)> AuthenticateAsync(string username, string clientEncryptedPassword);
    Task<(bool Success, string ErrorMessage)> RegisterAsync(string username, string email, string clientEncryptedPassword, string roleName);
    Task<User?> GetUserByUsernameOrEmailAsync(string identifier);
    Task<(bool Success, string ErrorMessage)> ResetPasswordAsync(string identifier, string newClientEncryptedPassword);
}
