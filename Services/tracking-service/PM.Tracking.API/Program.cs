using PM.Tracking.Application;
using PM.Shared.Swagger;
var builder = WebApplication.CreateBuilder(args);
builder.Services.InitializeSwagger(builder.Configuration,"PM.Tracking.API");
builder.Services.InitializeApplication(builder.Configuration);
var app = builder.Build();

if(app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger(options => { });
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "PM.Tracking.API v1");
        options.RoutePrefix = string.Empty;
    });
}
else
{
    
    app.UseExceptionHandler("/error");
    app.UseHsts();
}
app.MapControllers();
app.UseRouting();
app.UseAuthorization();
app.Run();
