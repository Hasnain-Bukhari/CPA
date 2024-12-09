using System.Diagnostics;
using Microsoft.Extensions.Options;

namespace CPA_RoundRobin.Services;
public class HealthCheckService : BackgroundService
{
    private readonly ILogger<HealthCheckService> _logger;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly List<string> _appInstances;
    public static List<string> HealthyInstances { get; private set; } = new();

    public HealthCheckService(IHttpClientFactory httpClientFactory, ILogger<HealthCheckService> logger, IOptions<ApplicationSettings> settings)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _appInstances = settings.Value.AppInstances;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Performing health check...");
            var healthyInstances = new List<string>();

            foreach (var instance in _appInstances)
            {
                if (await IsInstanceHealthy(instance))
                {
                    healthyInstances.Add(instance);
                    _logger.LogInformation($"Instance {instance} is healthy.");
                }
                else
                {
                    _logger.LogWarning($"Instance {instance} is unhealthy.");
                }
            }

            HealthyInstances = healthyInstances;

            // Wait for the next health check - 5 seconds interval
            await Task.Delay(5000, stoppingToken);
        }
    }

    private async Task<bool> IsInstanceHealthy(string instance)
    {
        try
        {
            var client = _httpClientFactory.CreateClient("RoundRobinClient");
            var timer = Stopwatch.StartNew();
            var response = await client.GetAsync(instance);
            timer.Stop();
            return response.IsSuccessStatusCode && timer.ElapsedMilliseconds < 3000;
        }
        catch
        {
            return false;
        }
    }
}