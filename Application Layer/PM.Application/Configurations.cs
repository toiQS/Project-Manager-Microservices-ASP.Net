using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PM.Application.Implements;
using PM.Application.Interfaces;
using PM.Infrastructers;
using PM.Persistence;

namespace PM.Application
{
    public static class Configurations
    {
        public static void IntializeApplication(this IServiceCollection services, IConfiguration configuration)
        {
            RegisterServices(services, configuration);
            services.InitializePersistence(configuration);
            services.InitializeInfrastructure(configuration);
        }
        private static void RegisterServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IAuthLogic, AuthLogic>();
            services.AddScoped<IDocumentLogic, DocumentLogic>();
            services.AddScoped<IMemberLogic, MemberLogic>();
            services.AddScoped<IMissionLogic, MissionLogic>();
            services.AddScoped<IPlanLogic, PlanLogic>();
            services.AddScoped<IReportLogic, ReportLogic>();
            services.AddScoped<IUserLogic, UserLogic>();
            services.AddScoped<IProjectLogic, ProjectLogic>();
            services.AddScoped<IMemberLogic, MemberLogic>();
            
        }
    }
}
