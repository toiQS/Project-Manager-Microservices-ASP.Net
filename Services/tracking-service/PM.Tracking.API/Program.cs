using PM.Tracking.Application;
var builder = WebApplication.CreateBuilder(args);

builder.Services.InitializeApplication(builder.Configuration);
var app = builder.Build();



app.Run();
