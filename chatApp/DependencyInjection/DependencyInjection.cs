using ChatApp.Repositories;
using ChatApp.Repositories.Interfaces;
using ChatApp.Services;
using ChatApp.Services.Interfaces;
using System.Data;
using Microsoft.Data.SqlClient; 

namespace ChatApp.DependencyInjection
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddProjectDependencies(this IServiceCollection services, string connectionString)
        {
            // Register Dapper connection
            services.AddScoped<IDbConnection>(sp =>     new SqlConnection(connectionString));

            // Repository Layer
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IMessageRepository, MessageRepository>();
            services.AddScoped<IConnectionRepository, ConnectionRepository>();

            // Service Layer
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IMessageService, MessageService>();
            services.AddScoped<IConnectionService, ConnectionService>();

            return services;
        }
    }
}
