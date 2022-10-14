using Scheduling.Domain.Enums;

namespace Scheduling.Domain.Models;

public class AssignmentScheduleHours
{
    public int AvailableSpots { get; set; } = 0;
    public int Day { get; set; }
    public int StartTime { get; set; }
    public int EndTime { get; set; }
}