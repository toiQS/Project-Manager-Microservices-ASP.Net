using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PM.Identity.Entities;
using PM.Identity.Infrastructure.Data;
using PM.Shared.Config;

namespace PM.Identity.Application
{
    public static class Config
    {
        public static void InitializeApplication(this IServiceCollection services, IConfiguration configuration)
        {
            services.Initialize<AuthDbContext>(configuration);
            services.RegisterIdentity(configuration);
        }
        private static void InitializeUnitOfWork(IServiceCollection services, IConfiguration configuration)
        {
            //services.AddScoped<IUnitOfWork, UnitOfWork>();
        }
        private static void InitializeRepositories(IServiceCollection services, IConfiguration configuration)
        {
           
        }
        private static void InitializeServices(IServiceCollection services, IConfiguration configuration)
        {
            
        }
        private static void RegisterIdentity(this IServiceCollection services, IConfiguration configuration)
        {
            
            services.AddIdentity<AppUser, IdentityRole>(options =>
            {
                options.Password.RequiredLength = 3;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.User.RequireUniqueEmail = true;

            }).AddEntityFrameworkStores<AuthDbContext>()
           .AddDefaultTokenProviders();

            services.Configure<IdentityOptions>(options =>
            {
                // Default Password settings.
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 3;
                options.Password.RequiredUniqueChars = 1;
            });
        }

    }
}
