using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PM.Domain.Entities;
using PM.Domain.Interfaces;
using PM.Domain.Interfaces.Services;
using PM.Persistence.Implements;
using PM.Persistence.Implements.Services;

namespace PM.Persistence
{
    public static class Configurations
    {
        public static void InitializePersistence(this IServiceCollection services, IConfiguration configuration)
        {
            RegisterDatabase(services, configuration);
            RegisterServices(services, configuration);
        }
        private static void RegisterDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("ProjectManagerConnectString"));
                options.EnableSensitiveDataLogging();
            });
        }
        private static void RegisterServices(this IServiceCollection services, IConfiguration configuration)
        {
            //services.AddScoped<IMemoryCache, MemoryCache>();
            //services.AddScoped<IEventBus, EventBus>();

            

            services.AddScoped<IQueryRepository<Document, string>, QueryRepository<Document, string>>();
            services.AddScoped<IQueryRepository<Mission, string>, QueryRepository<Mission, string>>();
            services.AddScoped<IQueryRepository<MissionAssignment, string>, QueryRepository<MissionAssignment, string>>();
            services.AddScoped<IQueryRepository<Plan, string>, QueryRepository<Plan, string>>();
            services.AddScoped<IQueryRepository<ProgressReport, string>, QueryRepository<ProgressReport, string>>();
            services.AddScoped<IQueryRepository<Project, string>, QueryRepository<Project, string>>();
            services.AddScoped<IQueryRepository<RoleInProject, string>, QueryRepository<RoleInProject, string>>();
            services.AddScoped<IQueryRepository<Status, string>, QueryRepository<Status, string>>();

            services.AddScoped<ICommandRepository<Document, string>, CommandRepository<Document, string>>();
            services.AddScoped<ICommandRepository<Mission, string>, CommandRepository<Mission, string>>();
            services.AddScoped<ICommandRepository<MissionAssignment, string>, CommandRepository<MissionAssignment, string>>();
            services.AddScoped<ICommandRepository<Plan, string>, CommandRepository<Plan, string>>();
            services.AddScoped<ICommandRepository<ProgressReport, string>, CommandRepository<ProgressReport, string>>();
            services.AddScoped<ICommandRepository<Project, string>, CommandRepository<Project, string>>();
            services.AddScoped<ICommandRepository<RoleInProject, string>, CommandRepository<RoleInProject, string>>();
            services.AddScoped<ICommandRepository<Status, string>, CommandRepository<Status, string>>();

            services.AddScoped<IProjectManagerUnitOfWork, ProjectManagerUnitOfWork>();

            services.AddScoped<IDocumentServices, DocumentServices>();
            services.AddScoped<IMissionServices, MissionServices>();
            services.AddScoped<IMissionAssignmentServices, MissionAssignmentServices>();
            services.AddScoped<IPlanServices, PlanMissionServices>();
            services.AddScoped<IProgressReportServices, ProgressReportServices>();
            services.AddScoped<IProjectServices, ProjectServices>();
            services.AddScoped<IRoleInProjectServices, RoleInProjectServices>();
            services.AddScoped<IStatusServices, StatusServices>();
        }
    }
}
