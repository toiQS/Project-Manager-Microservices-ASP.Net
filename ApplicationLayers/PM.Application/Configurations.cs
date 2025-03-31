using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PM.Application.Interfaces;
using PM.Application.Services;
using PM.Infrastructer;
using PM.Persistence;

namespace PM.Application
{
    public static class Configurations
    {
        public static void InitializeFlowLogic(this IServiceCollection services, IConfiguration configuration)
        {
            services.InitializePersistence(configuration);
            services.InitializeInfrastructer(configuration);
            RegisterFlowLogic(services, configuration);
        }
        private static void RegisterFlowLogic(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IAuthFlowLogic, AuthFlowLogic>();
            services.AddScoped<IUserFlowLogic, UserFlowLogic>();
        }
    }
}
