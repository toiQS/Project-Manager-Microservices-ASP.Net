using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PM.Shared.Config;
using PM.Shared.Handle.Implements;
using PM.Shared.Handle.Interfaces;
using PM.Tracking.Application.Implements;
using PM.Tracking.Application.Interfaces;
using PM.Tracking.Infrastructure.Data;

namespace PM.Tracking.Application
{
    public static class Config
    {
        public static void InitializeApplication(this IServiceCollection services, IConfiguration configuration)
        {
            services.InitializeSQL<TrackingDbContext>(configuration, "TrackingConnectString");
            services.InitializeRepositories(configuration);
            services.InitializeUnitOfWork(configuration);
            services.InitializeHandle(configuration);
        }
        private static void InitializeUnitOfWork(this IServiceCollection services, IConfiguration configuration)
        {

            services.AddScoped<IUnitOfWork<TrackingDbContext>, UnitOfWork<TrackingDbContext>>();
        }

        private static void InitializeRepositories(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped(typeof(IRepository<,>), typeof(Repository<,>));
        }
        private static void InitializeHandle(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<ITrackingHandle, TrackingHandle>();
        }


    }
}
