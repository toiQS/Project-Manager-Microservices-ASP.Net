using Microsoft.AspNetCore.Identity;
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
            RegisterIdentity(services, configuration);
        }
        public static void RegisterDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DockerConnectString"));
                options.EnableSensitiveDataLogging();
            });
        }
        private static void RegisterServices(this IServiceCollection services, IConfiguration configuration)
        {
            //repository
            services.AddScoped<IRepository<ActivityLog, string>, Repository<ActivityLog, string>>();
            services.AddScoped<IRepository<Document, string>, Repository<Document, string>>();
            services.AddScoped<IRepository<ProjectMember, string>, Repository<ProjectMember, string>>();
            services.AddScoped<IRepository<Project, string>, Repository<Project, string>>();
            services.AddScoped<IRepository<Mission, string>, Repository<Mission, string>>();
            services.AddScoped<IRepository<User, string>, Repository<User, string>>();
            services.AddScoped<IRepository<MissionAssignment, string>, Repository<MissionAssignment, string>>();
            services.AddScoped<IRepository<ProgressReport, string>, Repository<ProgressReport, string>>();
            services.AddScoped<IRepository<RoleInProject, string>, Repository<RoleInProject, string>>();
            services.AddScoped<IRepository<Status, int>, Repository<Status, int>>();
            services.AddScoped<IRepository<Plan, string>, Repository<Plan, string>>();

            services.AddScoped<UserManager<User>>();

            //unit of work
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            //services
            services.AddScoped<IAuthServices, AuthServices>();
            services.AddScoped<IProjectServices, ProjectServices>();
            services.AddScoped<IPlanServices, PlanServices>();
            services.AddScoped<IMemberServices, MemberServices>();
            services.AddScoped<IUserServices, UserServices>();
        }
        private static void RegisterIdentity(this IServiceCollection services, IConfiguration configuration)
        {
            //services.AddIdentity<User, IdentityRole>()
            //    .AddEntityFrameworkStores<ApplicationDbContext>()
            //    .AddDefaultTokenProviders();
            services.AddIdentity<User, IdentityRole>(options =>
            {
                options.Password.RequiredLength = 3;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.User.RequireUniqueEmail = true;

            }).AddEntityFrameworkStores<ApplicationDbContext>()
           .AddDefaultTokenProviders();

            services.Configure<IdentityOptions>(options =>
            {
                // Default Password settings.
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 1;
            });
        }
    }
}
