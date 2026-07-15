using Microsoft.EntityFrameworkCore;
using WeatherAPI.Context;
using WeatherAPI.Middleware;
using WeatherAPI.Service;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddHttpClient<WeatherService>(client =>
{
    client.Timeout = TimeSpan.FromSeconds(60);

    client.DefaultRequestHeaders.UserAgent.ParseAdd(
        "WeatherAPI/1.0");
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseMiddleware<Exceptionsm>();

// Swagger sempre disponível durante os testes
app.UseSwagger();
app.UseSwaggerUI();

// Removido temporariamente para evitar problemas com HTTPS
// app.UseHttpsRedirection();

app.MapControllers();

app.MapGet("/health", () =>
{
    return Results.Ok(new
    {
        status = "healthy"
    });
});

app.Run();