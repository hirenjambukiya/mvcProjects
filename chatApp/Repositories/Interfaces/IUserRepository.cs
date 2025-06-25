using System.Collections.Generic;
using System.Threading.Tasks;
using chatApp.Models;

namespace ChatApp.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetUserByIdAsync(Guid id);
        Task<User?> GetUserByNameAsync(string username);
        Task<IEnumerable<User>> GetAllUsersExceptSelfAsync(Guid userId);
        Task<int> CreateUserAsync(User user);
        Task UpdateOnlineStatusAsync(Guid userId, bool isOnline);
        Task UpdateLastActiveAsync(Guid userId);
    }
}
