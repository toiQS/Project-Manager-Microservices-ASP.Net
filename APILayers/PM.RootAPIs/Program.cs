using PM.Infrastructer;
using PM.Persistence;
using PM.RootAPIs.SeedData;

var builder = WebApplication.CreateBuilder(args);
builder.Services.InitializePersistence(builder.Configuration);
builder.Services.InitializeInfrastructer(builder.Configuration);
//builder.WebHost.UseUrls("https://127.0.0.1:7300");

// Add services to the container.

//builder.Services.InitializePersistence(builder.Configuration);
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await RoleSeeding.Initialize(services);
}

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await StatusSeeding.Initialize(services);
}

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await RoleInProjectSeeding.Initialize(services);
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
