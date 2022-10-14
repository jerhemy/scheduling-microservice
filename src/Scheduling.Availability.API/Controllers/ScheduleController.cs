using Microsoft.AspNetCore.Mvc;
using Scheduling.Availability.API.Models;

namespace Scheduling.Availability.API.Controllers;

public class ScheduleController : ControllerBase
{
    private readonly ILogger<ScheduleController> _logger;

    public ScheduleController(ILogger<ScheduleController> logger)
    {
        _logger = logger;
    }
}