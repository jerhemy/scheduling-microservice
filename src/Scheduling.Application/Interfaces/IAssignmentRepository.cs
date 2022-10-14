using Scheduling.Domain.Entities;

namespace Scheduling.Infrastructure.Interfaces;

public interface IAssignmentRepository
{
    Task<AssignmentEntity> CreateAsync(AssignmentEntity assignment);
    Task<void> DeleteAsync(string id);
}