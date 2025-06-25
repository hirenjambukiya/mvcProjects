using System.Collections.Generic;
using System.Threading.Tasks;
using chatApp.Models;

namespace ChatApp.Services.Interfaces
{
    public interface IUserService
    {
        Task<User?> GetUserByIdAsync(Guid id);
        Task<User?> GetUserByNameAsync(string username);
        Task<IEnumerable<User>> GetAllExceptSelfAsync(Guid currentUserId);
        Task<int> RegisterAsync(User user);
        Task UpdateOnlineStatusAsync(Guid userId, bool isOnline);
        Task UpdateLastActiveAsync(Guid userId);
    }
}
