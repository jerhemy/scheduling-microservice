namespace Scheduling.Reservation.Models;

public class ReservationFilteredRequest
{
    public string ProductGroup { get; set; }
    public string Account { get; set; }
    public DateTimeOffset? StartTime { get; set; }
    public DateTimeOffset? EndTime { get; set; }
    
    public List<string> Customers { get; set; }
    public List<string> Locations { get; set; }
    public List<string> Employees { get; set; }
    public List<string> Services { get; set; }
    public List<string> Resources { get; set; }
    public List<string> Rooms { get; set; }
    public List<string> Routes { get; set; }
    public int? Status { get; set; }
    public int? Type { get; set; }
}
