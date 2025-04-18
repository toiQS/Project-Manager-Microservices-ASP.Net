namespace PM.Shared.Config
{
    public static class ConfigService

    {
        public static void Initialize<TContext>(IServiceCollection services, IConfiguration configuration) where TContext : DbContext
        {
            services.InitializeSQL<TContext>(configuration);
        }
        public static void InitializeSQL<TContext>(this IServiceCollection services, IConfiguration configuration) where TContext : DbContext
        {
            services.AddDbContext<TContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            });
        }

    }

    // Move the extension method to a non-generic static class
    public static class ServiceCollectionExtensions
    {
       
    }
}
