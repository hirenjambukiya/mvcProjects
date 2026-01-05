namespace ex_APIarchitecture.Configurations
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddProjectDependencies(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddControllers();

            #region Swaagger
            services.AddEndpointsApiExplorer();
             services.AddSwaggerGen();
            #endregion

            #region HttpClient / Services / Repositories

            services.AddHttpClient();
            #endregion

            return services;
        }

        public static IApplicationBuilder UseProjectDependencies(this IApplicationBuilder app, IConfiguration configuration, IHostEnvironment env)
        {

            return app;
        }
    }
}
