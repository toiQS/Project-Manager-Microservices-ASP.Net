using Microsoft.AspNetCore.Authentication.JwtBearer;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);
Action<JwtBearerOptions> options = o =>
{
    o.Authority = "https://localhost:7193";
    o.Audience = "project-management";
};
builder.Services.AddAuthentication().AddJwtBearer("Bearer", options);
builder.Configuration.AddJsonFile("Ocelot.json", optional: false, reloadOnChange: true);
builder.Services.AddOcelot(builder.Configuration);

var app = builder.Build();

app.UseAuthentication();
app.UseOcelot().Wait();


app.Run();
