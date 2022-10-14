using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Scheduling.Domain.Enums;
using Scheduling.Domain.Models;

namespace Scheduling.Domain.Entities;

public class ReservationEntity
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    public string Account { get; set; }
    public string ProductGroup { get; set; }
    
    public DateTimeOffset StartTime { get; set; }
    public DateTimeOffset EndTime { get; set; }

    public int Duration { get; set; }

    public string Timezone { get; set; }
    
    public int Spots { get; set; }
    
    public ReservationStatus Status { get; set; }
    public ReservationType Type { get; set; }
    
    public string[] Assignments { get; set; }
    
    public List<AssignmentSummary> AssignmentDetails { get; set; }

    // TODO: Remove static method
    public static ReservationEntity FromReservation(ReservationItem reservationItems)
    {
        return new ReservationEntity
        {
            Id = reservationItems.Id,
            Assignments = reservationItems.Assignments,
            Duration = reservationItems.Duration,
            Spots = reservationItems.Spots,
            Status = reservationItems.Status,
            Type = reservationItems.Type,
            AssignmentDetails = reservationItems.AssignmentDetails,
            StartTime = reservationItems.StartTime,
            Account = reservationItems.Account,
            ProductGroup = reservationItems.ProductGroup
        };
    }
}