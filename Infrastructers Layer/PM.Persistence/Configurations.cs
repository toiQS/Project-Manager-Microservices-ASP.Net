using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PM.Domain.Entities;
using PM.Domain.Interfaces;
using PM.Domain.Interfaces.Services;
using PM.Persistence.Implements;
using PM.Persistence.Implements.Services;
using System.Security.Permissions;

namespace PM.Persistence
{
    public static class Configurations
    {
        public static void InitializeInfrastructer(this IServiceCollection services, IConfiguration configuration)
        {
            RegisterDatabase(services, configuration);
            RegisterServices(services, configuration);
        }
        public static void RegisterDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DockerConnectString")));
        }
        private static void RegisterServices(this IServiceCollection services, IConfiguration configuration)
        {
            //repository
            services.AddScoped<IRepository<ActivityLog, string>, Repository<ActivityLog, string>>();
            services.AddScoped<IRepository<Document, string>, Repository<Document,string>>();
            services.AddScoped<IRepository<ProjectMember, string>, Repository<ProjectMember, string>>();
            services.AddScoped<IRepository<Project,string> , Repository<Project, string>>();
            services.AddScoped<IRepository<Mission, string>, Repository<Mission, string>>();
            services.AddScoped<IRepository<User, string>, Repository<User, string>>();
            services.AddScoped<IRepository<MissionAssignment, string>, Repository<MissionAssignment, string>>();
            services.AddScoped<IRepository<ProgressReport, string>, Repository<ProgressReport, string>>();
            services.AddScoped<IRepository<RoleInProject,string>, Repository<RoleInProject, string>>();
            services.AddScoped<IRepository<Status,int>, Repository<Status, int>>();
            services.AddScoped<IRepository<Plan, string>, Repository<Plan, string>>();

            //services
            services.AddScoped<IProjectServices, ProjectServices>();
            services.AddScoped<IPlanServices, PlanServices>();
            services.AddScoped<IMemberServices, MemberServices>();
        }
    }
}
