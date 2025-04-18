namespace PM.Shared.Swagger
{
    public static class SetupSwagger
    {
        public static void InitializeSwagger(this IServiceCollection services, IConfiguration configuration, string title)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = title, Version = "v1" });
                c.SwaggerDoc("v2", new Microsoft.OpenApi.Models.OpenApiInfo { Title = title, Version = "v2" });
                c.EnableAnnotations(); // Nếu bạn dùng [SwaggerOperation], v.v.
                c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
            });
            services.AddEndpointsApiExplorer();
            services.AddControllers();

        }
    }
}
