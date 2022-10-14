using Scheduling.Domain.Models;

namespace Scheduling.Application.Interfaces;

public interface IReservationService
{
    Task<ReservationItem> CreateAsync(ReservationItem reservation);
    Task<List<ReservationItem>> CreateAsync(List<ReservationItem> reservations);
    
    Task<ReservationItem> GetByIdAsync(string id);
    Task<List<ReservationItem>> GetAsync(string productGroup, string account);
    
    Task<ReservationItem> UpdateAsync(ReservationItem reservationDbo);
    Task<ReservationItem> UpdateStatusAsync(string id, int status);
    
    Task RemoveAsync(string id);

    Task<List<ReservationItem>> FilterAsync(
        string productGroup,
        string account,
        DateTimeOffset? startTime,
        DateTimeOffset? endTime,
        List<ReservationTypeFilter> typeFilters,
        int? status,
        int? type
    );
}
