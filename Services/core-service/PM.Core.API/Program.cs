using PM.Core.Application;
using PM.Core.Infrastructure.Data;
var builder = WebApplication.CreateBuilder(args);

builder.Services.InitializeApplication(builder.Configuration);
var app = builder.Build();



app.Run();
