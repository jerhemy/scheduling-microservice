using Scheduling.Domain.Models;

namespace Scheduling.Application.Interfaces;

public interface IAssignmentService
{
    public Task<Assignment> GetAssignmentAsync(string assignmentId);
    
    public Task<Assignment> CreateAssignmentAsync(Assignment assignment);
    
    public Task<Assignment> UpdateAssignmentAsync(Assignment assignment);
    
    public Task<Assignment> DeleteAssignmentAsync(string assignmentId);
    
}