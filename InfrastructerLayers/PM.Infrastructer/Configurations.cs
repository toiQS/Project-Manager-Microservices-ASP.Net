using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace PM.Infrastructer
{
    public static class Configurations
    {
        public static void InitializeInfrastructer(this IServiceCollection services, IConfiguration configuration)
        {
            services.RegisterDatabase(configuration);
        }
        private static void RegisterDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AuthDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("AuthConnectString"));
                options.EnableSensitiveDataLogging();
            });
        }
        private static void RegisterServices(this IServiceCollection services, IConfiguration configuration)
        {

        }
    }
}
