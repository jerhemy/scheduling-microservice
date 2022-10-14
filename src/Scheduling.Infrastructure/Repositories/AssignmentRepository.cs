using MongoDB.Bson;
using MongoDB.Driver;
using Scheduling.Domain.Entities;
using Scheduling.Infrastructure.Interfaces;

namespace Scheduling.Infrastructure.Repositories;

public class AssignmentRepository : IAssignmentRepository
{
    private readonly IMongoCollection<AssignmentEntity> _collection;

    public AssignmentRepository(IMongoCollection<AssignmentEntity> collection)
    {
        _collection = collection;
    }
    
    public async Task<AssignmentEntity> CreateAsync(AssignmentEntity assignment)
    {
        await _collection.InsertOneAsync(assignment);
        return assignment;
    }
    
    public async Task<AssignmentEntity?> DeleteAsync(string id)
    {
        var result = await _collection.DeleteOneAsync(x => x.Id == id);
        return result.DeletedCount > 0 ? new AssignmentEntity { Id = id } : null;
    }

    public Task<bool> Validate(string id)
    {
        return Task.FromResult(_collection.FindAsync(x => x.Id == id).Result.ToList().Count > 0);
        
    }
}
