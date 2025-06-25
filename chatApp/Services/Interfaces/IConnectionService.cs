using System.Threading.Tasks;
using chatApp.Models;

namespace ChatApp.Services.Interfaces
{
    public interface IConnectionService
    {
        Task AddOrUpdateConnectionAsync(Guid userId, string connectionId);
        Task RemoveConnectionAsync(Guid userId);
        Task<Connection?> GetConnectionByUserIdAsync(Guid userId); 
    }
}
