using System.Text;
using System.Text.Json;
using CPA_RoundRobin.Services;
using Microsoft.AspNetCore.Mvc;

namespace CPA_RoundRobin.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RoundRobinController : ControllerBase
{
    private static int _currentIndex = 0;

    private readonly HttpClient _httpClient;

    public RoundRobinController(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient();
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] object payload)
    {
        var healthyInstances = HealthCheckService.HealthyInstances;
        if (healthyInstances.Count == 0)
        {
            Console.WriteLine($"No Active Instance found");
            return StatusCode(503, new { error = "All instances of application is down." });
        }

        var availableInstance = GetNextInstance(healthyInstances);
        Console.WriteLine($"Rerouting request to:{availableInstance}");
        Console.WriteLine($"Rerouting request with payload:{JsonSerializer.Serialize(payload)}");
        var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

        try
        {
            var response = await _httpClient.PostAsync(availableInstance, content);
            var responseContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Response from {availableInstance} -> :{JsonSerializer.Serialize(responseContent)}");
            return Content(responseContent, "application/json");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Request failed {ex.Message}");
            return StatusCode(500, new { error = ex.Message });
        }
    }

    private string GetNextInstance(List<string> availableInstances)
    {
        lock (availableInstances)
        {
            var instance = availableInstances[_currentIndex];
            _currentIndex = (_currentIndex + 1) % availableInstances.Count;
            return instance;
        }
    }
}