using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace CPA_JsonHandler.Controllers;

[ApiController]
[Route("api/[controller]")]
public class JsonHandlerController : ControllerBase
{
    [HttpGet]
    public IActionResult Get(){
        return Ok();
    }
    
    [HttpPost]
    public IActionResult Post([FromBody]object payload)
    {
        Console.WriteLine($"Request received with payload:{JsonSerializer.Serialize(payload)}");
        return Ok(payload);
    }  
}