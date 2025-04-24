using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PM.Shared.Config;
using PM.Tracking.Infrastructure.Data;

namespace PM.Tracking.Application
{
    public static class Config
    {
        public static void InitializeApplication(this IServiceCollection services, IConfiguration configuration)
        {
            services.InitializeSQL<TrackingDbContext>(configuration, "TrackingConnectString");
        }
    }
}
