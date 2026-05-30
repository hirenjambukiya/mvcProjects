using AMS.Application.DTOs;
using AMS.Application.Interfaces;
using AMS.Domain.Interfaces;
using AMS.Domain.Entities;
namespace AMS.Application.Services
{
    public class ApplicationService : IApplicationService
    {
        private readonly IApplicationRepository _repository;

        public ApplicationService(IApplicationRepository repository)
        {
            _repository = repository;
        }
        public async Task CreateAsync(ApplicationCreateDto dto, int userId)
        {
            try
            {
                Applications application = new()
                {
                    UserId = userId,
                    Name = dto.Name,
                    Age = dto.Age,
                    Gender = dto.Gender,
                    Country = dto.Country,
                    State = dto.State,
                    District = dto.District,
                    Pincode = dto.Pincode,
                    Address = dto.Address,
                    Status = "Pending"
                };

                if (dto.File != null)
                {
                    string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(),"wwwroot/uploads");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }
                    string uniqueFileName = Guid.NewGuid() + Path.GetExtension(dto.File.FileName);

                    string filePath = Path.Combine(uploadsFolder,uniqueFileName);

                    using FileStream stream = new(filePath, FileMode.Create);

                    await dto.File.CopyToAsync(stream);

                    application.Documents.Add(new ApplicationDocument
                    {
                        FileName = uniqueFileName,
                        OriginalFileName = dto.File.FileName,
                        FilePath = "/uploads/" + uniqueFileName
                    });
                }

                await _repository.AddAsync(application);

                await _repository.SaveChangesAsync();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<List<Applications>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<Applications?> GetByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<List<Applications>> GetByUserIdAsync(int userId)
        {
            try
            {
                return await _repository.GetByUserIdAsync(userId);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<DashboardDto> GetDashboardDataAsync()
        {
            try
            {
                var applications = await _repository.GetAllAsync();

                DashboardDto dashboard = new()
                {
                    TotalApplications = applications.Count,

                    PendingApplications = applications
                        .Count(x => x.Status == "Pending"),

                    ApprovedApplications = applications
                        .Count(x => x.Status == "Approved"),

                    RejectedApplications = applications
                        .Count(x => x.Status == "Rejected")
                };

                return dashboard;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<DashboardDto> GetMemberDashboardDataAsync(int userId)
        {
            try
            {
                var applications = await _repository.GetByUserIdAsync(userId);

                DashboardDto dashboard = new()
                {
                    TotalApplications = applications.Count,

                    PendingApplications = applications
                        .Count(x => x.Status == "Pending"),

                    ApprovedApplications = applications
                        .Count(x => x.Status == "Approved"),

                    RejectedApplications = applications
                        .Count(x => x.Status == "Rejected")
                };

                return dashboard;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task ReviewAsync(ApplicationReviewDto dto, int adminUserId)
        {
            var application =
            await _repository.GetByIdAsync(dto.ApplicationId);

            if (application == null)
                return;

            application.Status = dto.Status;

            application.Reviews.Add(new ApplicationReview
            {
                ReviewedByUserId = adminUserId,
                Status = dto.Status,
                Remarks = dto.Remarks
            });

            await _repository.SaveChangesAsync();
        }
    }
}
