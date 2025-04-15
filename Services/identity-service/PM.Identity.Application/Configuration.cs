using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PM.Identity.Application.Implements;
using PM.Identity.Application.Interfaces;
using PM.Identity.Domain.Entities;
using PM.Identity.Infrastructure.Config;
using PM.Identity.Infrastructure.Data;
using PM.Shared.Persistence.Implements;
using PM.Shared.Persistence.Interfaces;
using Microsoft.AspNetCore.Identity.UI;

namespace PM.Identity.Application
{
    public static class Configuration
    {
        public static void InitializeInfrastructureIdentity(this IServiceCollection services, IConfiguration configuration)
        {
            services.InitializeSQL(configuration);
            services.InitializeIdentity(configuration);
            services.InitializeRepository(configuration);
            services.InitializeServices(configuration);
            services.SeedingData(configuration).GetAwaiter().GetResult();
        }
        private static void InitializeRepository(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IRepository<AuthDbContext, User>, Repository<AuthDbContext, User>>();
            services.AddScoped<IRepository<AuthDbContext, RefreshToken>, Repository<AuthDbContext, RefreshToken>>();

            services.AddScoped<IUnitOfWork<AuthDbContext>, UnitOfWork<AuthDbContext>>();
        }
        private static void InitializeServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IUserService, UserService>();
            
            services.AddScoped<IRefreshTokenService, RefreshTokenService>();
        }
        private static async Task SeedingData(this IServiceCollection services, IConfiguration configuration)
        {
            var app = services.BuildServiceProvider();
            using (var scope = app.CreateScope())
            {
               var seedingResult = scope.ServiceProvider;
                await RoleSeeding.Initialize(seedingResult);
            }
        }
        private static void InitializeIdentity(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<IdentityOptions>(options =>
            {
                // Password settings
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 1;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
                // Lockout settings
                options.Lockout.AllowedForNewUsers = true;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                // User settings
                options.User.RequireUniqueEmail = true;
            });

            

            services.AddIdentity<User, IdentityRole>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.Password.RequiredLength = 1;
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
            })
                .AddEntityFrameworkStores<AuthDbContext>()
                .AddDefaultTokenProviders();
        }
    }
}
