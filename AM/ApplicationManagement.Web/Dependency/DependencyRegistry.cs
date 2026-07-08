using AMS.Application.Interfaces;
using AMS.Application.Services;
using AMS.Domain.Interfaces;
using AMS.Infrastructure.Repositories;

namespace AMS.Web.Dependency
{
    public class DependencyRegistry
    {
        private readonly IServiceCollection _services;

        public DependencyRegistry(IServiceCollection services)
        {
            _services = services;
        }

        public void RegisterDependencies()
        {
            RegisterServices();
            RegisterRepositories();
        }

        private void RegisterServices()
        {
            _services.AddScoped<IAuthService, AuthService>();
            _services.AddScoped<IApplicationService, ApplicationService>();
            _services.AddScoped<ILogService, LogService>();
        }

        private void RegisterRepositories()
        {
            _services.AddScoped<IUserRepository, UserRepository>();
            _services.AddScoped<IApplicationRepository, ApplicationRepository>();
        }

    }
}
