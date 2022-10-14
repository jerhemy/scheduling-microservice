using Scheduling.Domain.Enums;

namespace Scheduling.Domain.Models;

public class AssignmentScheduleHoursDbo
{
    // Defines Type of Schedule this is
    // Base,
    // Exception,
    // DayOn,
    // DayOff
    public ScheduleHoursType Type { get; set; }
    
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }

    public int AvailableSpots { get; set; } = 0;
    
    public int Day { get; set; }
    
    // Valid for Base and Exception Types Only
    public int? StartTime { get; set; }
    public int? EndTime { get; set; }
}