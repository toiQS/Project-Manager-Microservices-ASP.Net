using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace PM.Shared.Config
{
    public static class ConfigService

    {
        public static void Initialize<TContext>(this IServiceCollection services, IConfiguration configuration) where TContext : DbContext
        {
            services.InitializeSQL<TContext>(configuration);
        }
        public static void InitializeSQL<TContext>(this IServiceCollection services, IConfiguration configuration) where TContext : DbContext
        {
            services.AddDbContext<TContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DockerConnectString"));
            });
        }

    }

    // Move the extension method to a non-generic static class
    public static class ServiceCollectionExtensions
    {
       
    }
}
