using AMS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace AMS.Domain.Interfaces
{
    public interface IApplicationRepository
    {
        Task AddAsync(Applications application);
        Task<List<Applications>> GetByUserIdAsync(int userId);
        Task<List<Applications>> GetAllAsync();
        Task<Applications?> GetByIdAsync(int id);
        Task SaveChangesAsync();
    }
}
