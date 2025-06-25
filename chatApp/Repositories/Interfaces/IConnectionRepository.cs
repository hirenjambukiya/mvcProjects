using System.Threading.Tasks;
using chatApp.Models;

namespace ChatApp.Repositories.Interfaces
{
    public interface IConnectionRepository
    {
        Task<Connection?> GetConnectionByUserIdAsync(Guid userId);
        Task AddConnectionAsync(Connection connection);
        Task UpdateConnectionByConnectionIdAsync(Connection connection);
        Task RemoveConnectionByUserIdAsync(Guid userId);
    }
}
