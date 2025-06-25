using System.Reflection;
using chatApp.Models;
using ChatApp.Repositories.Interfaces;
using ChatApp.Services.Interfaces;

namespace ChatApp.Services
{
    public class ConnectionService : IConnectionService
    {
        private readonly IConnectionRepository _connectionRepository;

        public ConnectionService(IConnectionRepository connectionRepo)
        {
            _connectionRepository = connectionRepo;
        }

        public async Task AddOrUpdateConnectionAsync(Guid userId, string connectionId)
        {
            var existing = await _connectionRepository.GetConnectionByUserIdAsync(userId);

            if (existing == null)
            {
                await _connectionRepository.AddConnectionAsync(new Connection
                {
                    UserId = userId,
                    ConnectionId = connectionId,
                    ConnectedAt = DateTime.UtcNow
                });
            }
            else
            {
                existing.ConnectionId = connectionId;
                existing.ConnectedAt = DateTime.UtcNow;

                await _connectionRepository.UpdateConnectionByConnectionIdAsync(existing);
            }
        }

        public async Task RemoveConnectionAsync(Guid userId)
        {
            await _connectionRepository.RemoveConnectionByUserIdAsync(userId);
        }

        public async Task<Connection?> GetConnectionByUserIdAsync(Guid userId)
        {
            return await _connectionRepository.GetConnectionByUserIdAsync(userId);
        }
    }
}
