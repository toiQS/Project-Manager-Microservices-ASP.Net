using PM.Identity.Application;

var builder = WebApplication.CreateBuilder(args);



// Add services to the container
builder.Services.AddControllers();
builder.Services.InitializeInfrastructureIdentity(builder.Configuration);
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi



builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApiDocument();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
