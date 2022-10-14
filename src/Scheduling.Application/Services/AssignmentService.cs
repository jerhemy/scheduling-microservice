using Scheduling.Application.Interfaces;
using Scheduling.Domain.Entities;
using Scheduling.Domain.Models;
using Scheduling.Infrastructure.Interfaces;

namespace Scheduling.Infrastructure;

public class AssignmentService : IAssignmentService
{
    private readonly IAssignmentRepository _assignmentRepository;
    public AssignmentService(IAssignmentRepository assignmentRepository)
    {
        _assignmentRepository = assignmentRepository;
    }
    
    public async Task<Assignment> CreateAssignmentAsync(Assignment assignment)
    {
        var entity = new AssignmentEntity
        {
            Name = assignment.Name,
            Description = assignment.Description
        };
        
        var response = await _assignmentRepository.CreateAsync(entity);
        
        return new Assignment
        {
            Id = response.Id.ToString(),
            Description = response.Description
        };
    }
    
    public Task<Assignment> GetAssignmentAsync(string assignmentId)
    {
        throw new NotImplementedException();
    }
    public Task<Assignment> UpdateAssignmentAsync(Assignment assignment)
    {
        throw new NotImplementedException();
    }

    public Task<Assignment> DeleteAssignmentAsync(string assignmentId)
    {
        throw new NotImplementedException();
    }
}