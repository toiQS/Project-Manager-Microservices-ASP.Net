using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace PM.Persistence
{
    public static class Configurations
    {
        public static void InitializeInfrastructer(this IServiceCollection services, IConfiguration configuration)
        {
            RegisterDatabase(services, configuration);
        }
        public static void RegisterDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DockerConnectString")));
        }
    }
}
