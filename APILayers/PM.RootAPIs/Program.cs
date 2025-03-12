using PM.Infrastructer;
using PM.Persistence;

var builder = WebApplication.CreateBuilder(args);
builder.Services.InitializePersistence(builder.Configuration);
builder.Services.InitializeInfrastructer(builder.Configuration);
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

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
