using Meteostanice.Data;
using Meteostanice.Services;
using Meteostanice.Settings;
using Microsoft.Extensions.Options;

namespace Meteostanice;

public class Worker(
    ILogger<Worker> logger,
    MeteoFetcher fetcher,
    MeteoParser parser,
    IServiceScopeFactory scopeFactory,
    IOptions<MeteoSettings> settings)
    : BackgroundService
{
    private readonly MeteoSettings _settings = settings.Value;

    protected override async
        Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("MeteoCollector started");

        while (!stoppingToken.IsCancellationRequested)
        {
            await CollectDataAsync();
            await Task.Delay(
                TimeSpan.FromMinutes(_settings.IntervalMinutes), 
                stoppingToken);
        }
    }

    private async Task CollectDataAsync()
    {
        logger.LogInformation("Fetching meteo data at {Time}", DateTime.UtcNow);

        var record = new MeteoRecord
        {
            FetchedAt = DateTime.UtcNow,
            IsOnline = false,
            JsonData = null
        };

        try
        {
            var xml = await fetcher.FetchXmlAsync(_settings.XmlUrl);

            if (xml != null)
            {
                record.IsOnline = true;
                record.JsonData = parser.ParseXmlToJson(xml);
                logger.LogInformation("Data fetched and parsed successfully");
            }
        }
        catch (Exception ex)
        {
            logger.LogError("Meteo station unreachable: {Error}", ex.Message);
        }

        using var scope = scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<MeteoDbContext>();
        db.MeteoRecords.Add(record);
        await db.SaveChangesAsync();

        logger.LogInformation("Record saved. Online: {IsOnline}", record.IsOnline);
    }
}