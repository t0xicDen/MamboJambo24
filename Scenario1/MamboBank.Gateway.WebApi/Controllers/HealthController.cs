using Microsoft.AspNetCore.Mvc;

namespace MamboBank.Gateway.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new { Status = "Healthy", Service = "MamboBank Gateway API", Timestamp = DateTime.UtcNow });
    }
}