using chatApp.Models;
using ChatApp.Common;
using ChatApp.Repositories.Interfaces;
using ChatApp.Services.Interfaces;

namespace ChatApp.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepo;

        public UserService(IUserRepository userRepo)
        {
            _userRepo = userRepo;
        }

        public async Task<User?> GetUserByIdAsync(Guid id) => await _userRepo.GetUserByIdAsync(id);
        public async Task<User?> GetUserByNameAsync(string username) => await _userRepo.GetUserByNameAsync(username);
        public async Task<IEnumerable<User>> GetAllExceptSelfAsync(Guid currentUserId) => await _userRepo.GetAllUsersExceptSelfAsync(currentUserId);
        public async Task<int> RegisterAsync(User user)
        {
            User newUser = new User
            {
                UserId = Guid.NewGuid(),
                Username = user.Username,
                PasswordHash = PasswordHelper.HashPassword(user.PasswordHash),
                DisplayName = user.DisplayName,
                IsOnline = false,
                LastActive = DateTime.UtcNow
            };
            return await _userRepo.CreateUserAsync(newUser);
        }
        public async Task UpdateOnlineStatusAsync(Guid userId, bool isOnline) => await _userRepo.UpdateOnlineStatusAsync(userId, isOnline);
        public async Task UpdateLastActiveAsync(Guid userId) => await _userRepo.UpdateLastActiveAsync(userId);
    }
}
