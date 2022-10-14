using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Scheduling.Domain.Models;

namespace Scheduling.Domain.Entities;

public class AssignmentEntity
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    public string Account { get; set; }
    public string ProductGroup { get; set; }
    
    public string Name { get; set; }
    public string Description { get; set; }
    
    public ObjectId Parent { get; set; }
    
    public List<AssignmentSchedule> Schedules { get; set; }
    
    public List<AssignmentSchedule> Overrides { get; set; }

    public static AssignmentEntity FromAssignment(Assignment assignment)
    {
        return new AssignmentEntity
        {
            Id = assignment.Id,
            Name = assignment.Name,
            Description = assignment.Description,
            Parent = ObjectId.Parse(assignment.Parent),
            Schedules = assignment.Schedules,
        };
    }
}