using AMS.Application.DTOs;
using AMS.Domain.Entities;


namespace AMS.Application.Interfaces
{
    public interface IApplicationService
    {
        Task CreateAsync(ApplicationCreateDto dto, int userId);
        Task<List<Applications>> GetByUserIdAsync(int userId);
        Task<List<Applications>> GetAllAsync();
        Task<Applications?> GetByIdAsync(int id);
        Task ReviewAsync(ApplicationReviewDto dto, int adminUserId);
        Task<DashboardDto> GetDashboardDataAsync();
        Task<DashboardDto> GetMemberDashboardDataAsync(int userId);
    }
}
