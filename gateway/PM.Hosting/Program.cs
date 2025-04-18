var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.PM_Identity_API>("pm-identity-api");

builder.AddProject<Projects.PM_Web_API>("pm-web-api");

builder.Build().Run();
