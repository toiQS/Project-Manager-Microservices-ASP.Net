using PM.Identity.Application;
using PM.Shared.Swagger;
// Ensure correct namespace is included for SwaggerOptions

var builder = WebApplication.CreateBuilder(args);

builder.Services.InitializeSwagger(
    builder.Configuration,
    "PM.Identity.API"
);

builder.Services.AddHttpContextAccessor();

builder.Services.InitializeApplication(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger(options =>
    {
        // Configure SwaggerOptions explicitly to resolve ambiguity
    });
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "PM.Identity.API v1");
        c.SwaggerEndpoint("/swagger/v2/swagger.json", "PM.Identity.API v2");
        c.RoutePrefix = string.Empty; // Set Swagger UI at the app's root
    });
    app.UseStaticFiles();
}
else
{
    app.UseExceptionHandler("/error");
    app.UseHsts();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
