using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PM.Identity.Application.Implements;
using PM.Identity.Application.Interfaces;
using PM.Identity.Entities;
using PM.Identity.Infrastructure.Data;
using PM.Shared.Config;
using PM.Shared.Handle.Implements;
using PM.Shared.Handle.Interfaces;
using PM.Shared.Jwt;

namespace PM.Identity.Application
{
    public static class Config
    {
        public static void InitializeApplication(this IServiceCollection services, IConfiguration configuration)
        {
            services.Initialize<AuthDbContext>(configuration);
            services.RegisterIdentity(configuration);
            services.InitializeUnitOfWork(configuration); // Fixed method call
            services.InitializeRepositories(configuration); // Fixed method call
            services.InitializeServices(configuration); // Fixed method call
        }

        private static void InitializeUnitOfWork(this IServiceCollection services, IConfiguration configuration)
        {
           
           services.AddScoped<IUnitOfWork<AuthDbContext>, UnitOfWork<AuthDbContext>>(); 
        }

        private static void InitializeRepositories(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped(typeof(IRepository<,>), typeof(Repository<,>));
        }

        private static void InitializeServices(this IServiceCollection services, IConfiguration configuration) // Fixed method signature
        {
            services.AddScoped<IAuthHandle, AuthHandle>();
            services.AddScoped<IUserHandle, UserHandle>();

            services.AddScoped<IJwtService, JwtService>();
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
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 3;
                options.Password.RequiredUniqueChars = 1;
            });
        }
    }
}
