using System.Collections.Generic;
using System.Threading.Tasks;
using ApiTester.Domain.Entities;

namespace ApiTester.Domain.Repositories
{
    public interface IApiRequestHistoryRepository
    {
        Task<int> AddAsync(ApiRequestHistory history);
        Task<IEnumerable<ApiRequestHistory>> GetAllAsync();
        Task<ApiRequestHistory> GetByIdAsync(int id);
        Task<bool> DeleteAsync(int id);
        
        // Metrics for dashboard
        Task<int> GetTotalCallsAsync();
        Task<int> GetSuccessCountAsync();
        Task<int> GetFailedCountAsync();
        Task<double> GetAverageResponseTimeAsync();
        Task<IEnumerable<ApiRequestHistory>> GetRecentCallsAsync(int count);
    }
}
