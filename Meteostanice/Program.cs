using Meteostanice;
using Meteostanice.Api;
using Meteostanice.Data;
using Meteostanice.Services;
using Meteostanice.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Serilog
builder.Host.UseSerilog((context, config) =>
    config.ReadFrom.Configuration(context.Configuration));

// Settings
builder.Services.Configure<MeteoSettings>(
    builder.Configuration.GetSection("MeteoSettings"));

// Database
builder.Services.AddDbContext<MeteoDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Services
builder.Services.AddHttpClient<MeteoFetcher>();
builder.Services.AddSingleton<MeteoParser>();
builder.Services.AddHostedService<Worker>();

var app = builder.Build();

// Automatic migration
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<MeteoDbContext>();
    db.Database.Migrate();
}

// Endpointy
app.MapDashboardEndpoints();

app.Run();