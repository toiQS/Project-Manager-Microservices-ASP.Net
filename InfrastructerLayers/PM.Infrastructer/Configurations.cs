using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PM.Domain.Entities;
using PM.Domain.Interfaces;
using PM.Domain.Interfaces.Services;
using PM.Infrastructer.Implements;
using PM.Infrastructure.Implements.Services;

namespace PM.Infrastructer
{
    public static class Configurations
    {
        public static void InitializeInfrastructer(this IServiceCollection services, IConfiguration configuration)
        {
            services.RegisterDatabase(configuration);
            services.RegisterServices(configuration);
            services.RegisterCustom(configuration);
        }
        private static void RegisterDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AuthDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("AuthConnectString"));
                options.EnableSensitiveDataLogging();
            });
        }
        private static void RegisterServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<ICommandRepository<User, string>, CommandRepository<User, string>>();
            services.AddScoped<ICommandRepository<ActivityLog, string>, CommandRepository<ActivityLog, string>>();
            services.AddScoped<ICommandRepository<RefreshToken, string>, CommandRepository<RefreshToken, string>>();

            services.AddScoped<IQueryRepository<User, string>, QueryRepository<User, string>>();
            services.AddScoped<IQueryRepository<ActivityLog, string>, QueryRepository<ActivityLog, string>>();
            services.AddScoped<IQueryRepository<RefreshToken, string>, QueryRepository<RefreshToken, string>>();

            services.AddScoped<IAuthServices, AuthServices>();
            services.AddScoped<IAuthUnitOfWork, AuthUnitOfWork>();

            services.AddScoped<IActivityLogServices, ActivityLogServices>();
            services.AddScoped<IUserServices, UserServices>();
            services.AddScoped<IRefreshTokenServices, RefreshTokenServices>();
        }
        private static void RegisterCustom(this IServiceCollection services, IConfiguration configuration)
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

            }).AddEntityFrameworkStores<AuthDbContext>()
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
