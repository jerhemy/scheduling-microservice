using Scheduling.Domain.Models;

namespace Scheduling.Application.Models;

public class CreateReservationRequest
{
    public string Account { get; set; }
    public string ProductGroup { get; set; }

    public DateTimeOffset StartTime { get; set; }
    
    public int Duration { get; set; }
    public int Spots { get; set; }
    
    public int Type { get; set; }
    
    public string[] Assignments { get; set; }
    
    public List<AssignmentSummary> AssignmentDetails { get; set; }
}