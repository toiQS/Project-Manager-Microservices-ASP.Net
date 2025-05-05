using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PM.Core.Infrastructure.Data;
using PM.Shared.Config;
using PM.Shared.Handle.Implements;
using PM.Shared.Handle.Interfaces;


namespace PM.Core.Application
{
    public static class Config
    {
        public static void InitializeApplication(this IServiceCollection services, IConfiguration configuration)
        {
            services.InitializeSQL<CoreDbContext>(configuration, "CoreConnectString");
            services.InitializeRepositories(configuration);
            services.InitializeUnitOfWork(configuration);
        }
        private static void InitializeUnitOfWork(this IServiceCollection services, IConfiguration configuration)
        {

            services.AddScoped<IUnitOfWork<CoreDbContext>, UnitOfWork<CoreDbContext>>();
        }

        private static void InitializeRepositories(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped(typeof(IRepository<,>), typeof(Repository<,>));
        }
        private static void InitializeHandle(this IServiceCollection services, IConfiguration configuration)
        {

        }
    }
}
