using System.Data;
using chatApp.Models;
using ChatApp.Common;
using ChatApp.Repositories.Interfaces;
using Dapper;

namespace ChatApp.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IDbConnection _db;
        

        public UserRepository(IDbConnection db)
        {
            _db = db;
        }

        public async Task<User?> GetUserByIdAsync(Guid userId)
        {
            var parameters = new { UserId = userId };
            return await _db.QueryFirstOrDefaultAsync<User>("USP_GetUserById", parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task<User?> GetUserByNameAsync(string username)
        {
            var parameters = new { Username = username };
            return await _db.QueryFirstOrDefaultAsync<User>("USP_Get_User_ByUsername", parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<User>> GetAllUsersExceptSelfAsync(Guid userId)
        {
            var parameters = new { CurrentUserId = userId };
            return await _db.QueryAsync<User>("USP_GetAllUsersExceptSelf", parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task<int> CreateUserAsync(User user)
        {
            var parameters = new
            {
                user.UserId,
                user.Username,
                user.PasswordHash,
                user.DisplayName
            };
            return await _db.ExecuteScalarAsync<int>("USP_CreateUser", parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task UpdateOnlineStatusAsync(Guid userId, bool isOnline)
        {
            var parameters = new { UserId = userId, IsOnline = isOnline };
            await _db.ExecuteAsync("USP_UpdateUserStatus", parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task UpdateLastActiveAsync(Guid userId)
        {
            var parameters = new { UserId = userId };
            await _db.ExecuteAsync("USP_UpdateLastActive", parameters, commandType: CommandType.StoredProcedure);
        }
    }
}
