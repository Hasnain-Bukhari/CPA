using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CPA_RoundRobin.Services;
public class HealthCheckService : BackgroundService
{
    private readonly ILogger<HealthCheckService> _logger;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly List<string> _appInstances;
    public static List<string> HealthyInstances { get; private set; } = new();

    public HealthCheckService(IHttpClientFactory httpClientFactory, ILogger<HealthCheckService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _appInstances = new List<string>
        {
            "http://localhost:5041/api/jsonhandler",
            "http://localhost:5042/api/jsonhandler",
            "http://localhost:5043/api/jsonhandler",
            "http://localhost:5044/api/jsonhandler",
        };
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

            // Wait for the next health check (e.g., 10 seconds)
            await Task.Delay(5000, stoppingToken);
        }
    }

    private async Task<bool> IsInstanceHealthy(string instance)
    {
        try
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync(instance);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
}