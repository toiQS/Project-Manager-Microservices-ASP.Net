var builder = DistributedApplication.CreateBuilder(args);
builder.AddProject<Projects.PM_Identity_API>("pm-identity-api");

builder.AddProject<Projects.PM_Web_API>("pm-web-api");

builder.AddProject<Projects.PM_Gateway>("pm-gateway");

builder.AddProject<Projects.PM_Tracking_API>("pm-tracking-api");

builder.Build().Run();
