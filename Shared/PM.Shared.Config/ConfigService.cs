using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace PM.Shared.Config
{
    public static class ConfigService

    {
        public static void InitializeSQL<TContext>(this IServiceCollection services, IConfiguration configuration, string nameConnectString) where TContext : DbContext
        {
            services.AddDbContext<TContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString(nameConnectString));
            });
        }

    }
}
