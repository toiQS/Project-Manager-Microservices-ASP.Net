using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PM.Identity.Infrastructure.Config;

namespace PM.Identity.Application
{
    public static class Configuration
    {
        public static void InitializeInfrastructureIdentity(this IServiceCollection services, IConfiguration configuration)
        {
            services.InitializeSQL(configuration);
            services.InitializeIdentity();
            services.InitializeJwt(configuration);
        }
    }
}
