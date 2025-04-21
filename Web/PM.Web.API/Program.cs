using PM.Shared.Swagger;
var builder = WebApplication.CreateBuilder(args);
builder.Services.InitializeSwagger(
    builder.Configuration,
    "PM.Web.API"
);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    //app.MapSwagger();
    app.UseSwagger(options =>
    {
        // Configure SwaggerOptions explicitly to resolve ambiguity
    });
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "PM.Web.API v1");
        c.SwaggerEndpoint("/swagger/v2/swagger.json", "PM.Web.API v2");
        c.RoutePrefix = string.Empty; // Set Swagger UI at the app's root
    });
   
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
