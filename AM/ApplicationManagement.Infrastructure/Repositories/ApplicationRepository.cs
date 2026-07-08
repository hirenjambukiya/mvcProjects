using AMS.Domain.Entities;
using AMS.Domain.Interfaces;
using AMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMS.Infrastructure.Repositories
{
    public class ApplicationRepository : IApplicationRepository
    {
        private readonly ApplicationDbContext _context;
        public ApplicationRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(Applications application)
        {
            try
            {
                await _context.Applications.AddAsync(application);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<List<Applications>> GetAllAsync()
        {
            try
            {
                return await _context.Applications
                      .Include(x => x.Documents)
                      .Include(x => x.User)
                      .OrderByDescending(x => x.Id)
                      .ToListAsync();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<Applications?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Applications
                     .Include(x => x.Documents)
                     .Include(x => x.User)
                     .Include(x => x.Reviews)
                     .FirstOrDefaultAsync(x => x.Id == id);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<List<Applications>> GetByUserIdAsync(int userId)
        {
            try
            {
                return await _context.Applications
                    .Include(x => x.Documents)
                    .Where(x => x.UserId == userId)
                    .OrderByDescending(x => x.Id)
                    .ToListAsync();
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task SaveChangesAsync()
        {
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
