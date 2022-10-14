using Scheduling.Domain.Enums;

namespace Scheduling.Domain.Models;

public class AssignmentSchedule
{
    // Defines Type of Schedule this is
    // Base, Base Schedule for a given time period
    // Exception, Change of Hours for a Day
    // DayOn, Add a Day to a normally off day
    // DayOff, Remove a day from a normally on day
    public ScheduleHoursType Type { get; set; }
    
    // Used in Base, Exception and DayOn Types
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    
    public List<AssignmentScheduleHours> Hours { get; set; }
}