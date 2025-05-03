var builder = DistributedApplication.CreateBuilder(args);
builder.AddProject<Projects.PM_Identity_API>("pm-identity-api");

builder.AddProject<Projects.PM_Web_API>("pm-web-api");

builder.AddProject<Projects.PM_Gateway>("pm-gateway");

builder.AddProject<Projects.PM_Tracking_API>("pm-tracking-api");

builder.AddProject<Projects.PM_Core_API>("pm-core-api");

builder.Build().Run();
