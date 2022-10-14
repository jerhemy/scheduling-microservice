using Microsoft.AspNetCore.Mvc;
using Scheduling.Application.Interfaces;
using Scheduling.Application.Models;
using Scheduling.Availability.API.Models;
using Scheduling.Domain.Models;

namespace Scheduling.Availability.API.Controllers;

[ApiController]
[Route("[controller]")]
public class AssignmentController : ControllerBase
{
    private readonly ILogger<AssignmentController> _logger;
    private readonly IAssignmentService _assignmentService;

    public AssignmentController(ILogger<AssignmentController> logger, IAssignmentService assignmentService)
    {
        _logger = logger;
        _assignmentService = assignmentService;
    }

    [HttpGet]
    public Task<IActionResult> Get()
    {
        return Task.FromResult<IActionResult>(Ok());
    }

    [HttpPost]
    public Task<Assignment> CreateAssignment(CreateAssignmentRequest assignment)
    {
        // Validate Type and Properties
        
        
        var newAssignment = new Assignment()
        {
            Name = assignment.Name,
            Description = assignment.Description,
            Parent = assignment.Parent
        };
        
        return _assignmentService.CreateAssignmentAsync(newAssignment);
    }
    
    [HttpDelete]
    [Route("{id}")]
    public async Task<bool> DeleteAssignment(string id)
    {
        var response = await _assignmentService.DeleteAssignmentAsync(id);

        return await Task.FromResult(true);

    }
}