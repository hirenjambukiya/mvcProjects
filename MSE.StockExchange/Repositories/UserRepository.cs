using System;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using MSE.StockExchange.Data;
using MSE.StockExchange.Models.Domain;

namespace MSE.StockExchange.Repositories;

public class UserRepository : IUserRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public UserRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<User?> GetUserByUsernameAsync(string username)
    {
        using var connection = _connectionFactory.CreateConnection();
        var query = @"
            SELECT u.*, r.RoleName
            FROM Users u
            LEFT JOIN UserRoles ur ON u.Id = ur.UserId
            LEFT JOIN Roles r ON ur.RoleId = r.Id
            WHERE u.Username = @Username";

        var result = await connection.QueryAsync<User>(query, new { Username = username });
        return result.FirstOrDefault();
    }

    public async Task UpdateFailedAttemptAsync(int userId, int count, bool isLockedOut, DateTime? lockoutEnd)
    {
        using var connection = _connectionFactory.CreateConnection();
        var query = @"
            UPDATE Users 
            SET FailedAttemptCount = @Count, 
                IsLockedOut = @IsLockedOut, 
                LockoutEnd = @LockoutEnd
            WHERE Id = @UserId";

        await connection.ExecuteAsync(query, new { UserId = userId, Count = count, IsLockedOut = isLockedOut, LockoutEnd = lockoutEnd });
    }

    public async Task ResetFailedAttemptAsync(int userId)
    {
        using var connection = _connectionFactory.CreateConnection();
        var query = @"
            UPDATE Users 
            SET FailedAttemptCount = 0, 
                IsLockedOut = 0, 
                LockoutEnd = NULL
            WHERE Id = @UserId";

        await connection.ExecuteAsync(query, new { UserId = userId });
    }

    public async Task CreateUserAsync(User user, string roleName)
    {
        using var connection = _connectionFactory.CreateConnection();
        
        // Ensure role exists
        var roleId = await connection.QuerySingleOrDefaultAsync<int?>(
            "SELECT Id FROM Roles WHERE RoleName = @RoleName", new { RoleName = roleName });
            
        if (!roleId.HasValue) 
        {
            throw new Exception($"Role {roleName} does not exist.");
        }

        var insertUserQuery = @"
            INSERT INTO Users (Username, PasswordHash, Email, IsActive)
            VALUES (@Username, @PasswordHash, @Email, @IsActive);
            SELECT SCOPE_IDENTITY();";

        var newUserId = await connection.QuerySingleAsync<int>(insertUserQuery, user);

        var insertUserRoleQuery = @"
            INSERT INTO UserRoles (UserId, RoleId)
            VALUES (@UserId, @RoleId)";

        await connection.ExecuteAsync(insertUserRoleQuery, new { UserId = newUserId, RoleId = roleId.Value });
    }
}
