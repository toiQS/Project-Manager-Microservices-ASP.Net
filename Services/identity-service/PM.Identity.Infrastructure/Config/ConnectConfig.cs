using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PM.Identity.Infrastructure.Data;

namespace PM.Identity.Infrastructure.Config
{
    public static class ConnectConfig
    {
        public static void InitializeSQL(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AuthDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DockerConnectString"));
                options.EnableSensitiveDataLogging();
            });
        }
    }
}
