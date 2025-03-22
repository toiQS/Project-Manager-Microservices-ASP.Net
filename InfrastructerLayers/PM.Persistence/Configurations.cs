using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace PM.Persistence
{
    public static class Configurations
    {
        public static void InitializePersistence(this IServiceCollection services, IConfiguration configuration)
        {
            RegisterDatabase(services, configuration);
            RegisterServices(services, configuration);
        }
        private static void RegisterDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("ProjectManagerConnectString"));
                options.EnableSensitiveDataLogging();
            });
        }
        private static void RegisterServices(this IServiceCollection services, IConfiguration configuration)
        {
            //services.AddScoped<IMemoryCache, MemoryCache>();
            //services.AddScoped<IEventBus, EventBus>();
           
        }
    }
}
