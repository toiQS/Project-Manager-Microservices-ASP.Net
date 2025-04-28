using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using PM.Shared.Handle.Implements;
using PM.Shared.Handle.Interfaces;
using PM.Shared.Swagger;
using System.Text;
var builder = WebApplication.CreateBuilder(args);
builder.Services.InitializeSwaggerIdentity(
    builder.Configuration,
    "PM.Web.API"
);

builder.Services.AddScoped(typeof(IAPIService<>), typeof(APIService<>));
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddHttpContextAccessor();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = true;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"]))
        };
    });


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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
