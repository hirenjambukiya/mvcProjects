using System.Data;
using chatApp.Models;
using ChatApp.Repositories.Interfaces;
using Dapper;

public class ConnectionRepository : IConnectionRepository
{
    private readonly IDbConnection _db;

    public ConnectionRepository(IDbConnection db)
    {
        _db = db;
    }

    public async Task<Connection?> GetConnectionByUserIdAsync(Guid userId)
    {
        var result = await _db.QueryFirstOrDefaultAsync<Connection>(
            "USP_Get_Connection_ByUserId",
            new { UserId = userId },
            commandType: CommandType.StoredProcedure
        );

        return result;
    }

    public async Task AddConnectionAsync(Connection connection)
    {
        await _db.ExecuteAsync(
            "USP_Insert_Connection",
            new
            {
                connection.UserId,
                connection.ConnectionId,
                connection.ConnectedAt
            },
            commandType: CommandType.StoredProcedure
        );
    }

    public async Task UpdateConnectionByConnectionIdAsync(Connection connection)
    {
        await _db.ExecuteAsync(
            "USP_Update_Connection",
            new
            {
                connection.UserId,
                connection.ConnectionId,
                connection.ConnectedAt
            },
            commandType: CommandType.StoredProcedure
        );
    }

    public async Task RemoveConnectionByUserIdAsync(Guid userId)
    {
        await _db.ExecuteAsync(
            "USP_Delete_Connection_ByUserId",
            new { UserId = userId },
            commandType: CommandType.StoredProcedure
        );
    }
}
