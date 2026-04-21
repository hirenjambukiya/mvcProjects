using System.Threading.Tasks;
using MSE.StockExchange.Models.Domain;

namespace MSE.StockExchange.Repositories;

public interface IUserRepository
{
    Task<User?> GetUserByUsernameAsync(string username);
    Task UpdateFailedAttemptAsync(int userId, int count, bool isLockedOut, System.DateTime? lockoutEnd);
    Task ResetFailedAttemptAsync(int userId);
    Task CreateUserAsync(User user, string roleName);
}
