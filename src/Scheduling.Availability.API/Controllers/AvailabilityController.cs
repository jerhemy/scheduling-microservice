using Microsoft.AspNetCore.Mvc;

namespace Scheduling.Availability.API.Controllers;

[ApiController]
[Route("[controller]")]
public class AvailabilityController : ControllerBase
{
    private readonly ILogger<AvailabilityController> _logger;

    public AvailabilityController(ILogger<AvailabilityController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public Task<IActionResult> Get()
    {
        return Task.FromResult<IActionResult>(Ok());
    }
 
    
}