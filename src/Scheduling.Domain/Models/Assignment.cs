using Scheduling.Domain.Enums;

namespace Scheduling.Domain.Models;

public class Assignment
{
    public string Id { get; set; }
    
    public AssignmentType Type { get; set; }
    
    public string Name { get; set; }
    
    public string Description { get; set; }
    
    public string Account { get; set; }
    
    public string ProductGroup { get; set; }
    
    public string Parent { get; set; }

    public int Duration { get; set; } = 0;
    
    public List<AssignmentSchedule> Schedules { get; set; }
}