namespace Meteostanice.Services;

using Polly;
using Polly.Retry;

public class MeteoFetcher
{
    private readonly HttpClient _httpClient;
    private readonly AsyncRetryPolicy<string?> _retryPolicy;

    public MeteoFetcher(HttpClient httpClient, ILogger<MeteoFetcher> logger)
    {
        _httpClient = httpClient;

        _retryPolicy = Policy<string?>
            .Handle<HttpRequestException>()
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: attempt => TimeSpan.FromSeconds(attempt * 5),
                onRetry: (exception, delay, attempt, _) =>
                {
                    logger.LogWarning(
                        "Fetch attempt {Attempt} failed: {Error}. Retrying in {Delay}s...",
                        attempt, exception.Exception.Message, delay.TotalSeconds);
                });
    }

    public async Task<string?> FetchXmlAsync(string url)
    {
        return await _retryPolicy.ExecuteAsync(async () =>
        {
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        });
    }
}