var builder = DistributedApplication.CreateBuilder(args);
var identityService = builder.AddProject<Projects.PM_Identity_API>("identity-service");
var projectService = builder.AddProject<Projects.PM_Project_API>("project-service");
builder.Build().Run();
