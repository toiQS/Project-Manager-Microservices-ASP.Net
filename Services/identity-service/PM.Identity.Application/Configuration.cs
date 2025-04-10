using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PM.Identity.Application.Implements;
using PM.Identity.Application.Interfaces;
using PM.Identity.Domain.Entities;
using PM.Identity.Infrastructure.Config;
using PM.Identity.Infrastructure.Data;
using PM.Shared.Persistence.Implements;
using PM.Shared.Persistence.Interfaces;

namespace PM.Identity.Application
{
    public static class Configuration
    {
        public static void InitializeInfrastructureIdentity(this IServiceCollection services, IConfiguration configuration)
        {
            services.InitializeSQL(configuration);
            services.InitializeIdentity();
            services.InitializeRepository(configuration);
            services.InitializeServices(configuration);
            services.SeedingData(configuration).GetAwaiter().GetResult();
        }
        private static void InitializeRepository(this IServiceCollection serviceDescriptors, IConfiguration configuration)
        {
            serviceDescriptors.AddScoped<IRepository<AuthDbContext, User>, Repository<AuthDbContext, User>>();
            serviceDescriptors.AddScoped<IRepository<AuthDbContext, RefreshToken>, Repository<AuthDbContext, RefreshToken>>();

            serviceDescriptors.AddScoped<IUnitOfWork<AuthDbContext>, UnitOfWork<AuthDbContext>>();
        }
        private static void InitializeServices(this IServiceCollection serviceDescriptors, IConfiguration configuration)
        {
            serviceDescriptors.AddScoped<IAuthService, AuthService>();
            serviceDescriptors.AddScoped<IUserService, UserService>();
            
            serviceDescriptors.AddScoped<IRefreshTokenService, RefreshTokenService>();
        }
        private static async Task SeedingData(this IServiceCollection serviceDescriptors, IConfiguration configuration)
        {
            var app = serviceDescriptors.BuildServiceProvider();
            using (var scope = app.CreateScope())
            {
               var services = scope.ServiceProvider;
                await RoleSeeding.Initialize(services);
            }
        }
        
    }
}
