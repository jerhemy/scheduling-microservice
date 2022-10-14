using MongoDB.Bson;
using MongoDB.Driver;
using Scheduling.Domain.Models;
using Scheduling.Application.Interfaces;
using Scheduling.Domain.Entities;
using Scheduling.Infrastructure.Interfaces;

namespace Scheduling.Application.Services;

public class ReservationService : IReservationService
{
    private readonly IReservationRepository _reservationRepository;
    public ReservationService(IReservationRepository reservationRepository)
    {
        _reservationRepository = reservationRepository;
    }

    public async Task<ReservationItem> CreateAsync(ReservationItem reservation)
    {
        var obj = ReservationEntity.FromReservation(reservation);
        var result= await _reservationRepository.CreateAsync(obj);
        return ReservationItem.FromReservationEntity(result);
    }
    
    public Task<List<ReservationItem>> CreateAsync(List<ReservationItem> reservations)
    {
        throw new NotImplementedException();
    }

    public async Task<ReservationItem> GetByIdAsync(string id)
    {
        var result = await _reservationRepository.GetByIdAsync(id);
        return ReservationItem.FromReservationEntity(result);
    }

    public Task<List<ReservationItem>> GetAsync(string productGroup, string account)
    {
        throw new NotImplementedException();
    }

    public Task<ReservationItem> UpdateAsync(ReservationItem reservationDbo)
    {
        throw new NotImplementedException();
    }

    public async Task<ReservationItem> UpdateAsync(ReservationEntity reservationDbo)
    {
        var result = await _reservationRepository.UpdateAsync(reservationDbo);
        return ReservationItem.FromReservationEntity(result);
    }

    public async Task RemoveAsync(string id) =>
        await _reservationRepository.RemoveAsync(id);

    public async Task<ReservationItem> UpdateStatusAsync(string id, int status)
    {
        var result = await _reservationRepository.UpdateStatusAsync(id, status);
        return ReservationItem.FromReservationEntity(result);
    }

    public async Task<List<ReservationItem>> FilterAsync(
        string productGroup,
        string account,
        DateTimeOffset? startTime,
        DateTimeOffset? endTime,
        List<ReservationTypeFilter> typeFilters,
        int? status,
        int? type
    )
    {
        // Obtain these from token or whatever mechanism we will be using for service to service communication
        var filters = new List<FilterDefinition<ReservationEntity>>
        {
            Builders<ReservationEntity>.Filter.Eq("Account", account),
            Builders<ReservationEntity>.Filter.Eq("ProductGroup", productGroup),
        };

        if (startTime.HasValue)
        {
            filters.Add(Builders<ReservationEntity>.Filter.Gte("StartTime.DateTime", new BsonDateTime(startTime.Value.UtcDateTime)));
        }

        if (endTime.HasValue)
        {
            filters.Add(Builders<ReservationEntity>.Filter.Lte("EndTime.DateTime", new BsonDateTime(endTime.Value.UtcDateTime)));
            filters.Add(Builders<ReservationEntity>.Filter.Lte("StartTime.DateTime", new BsonDateTime(endTime.Value.UtcDateTime)));
        }
        
        if (typeFilters?.Count > 0)
        {
            foreach (var typeFilter in typeFilters)
            {
                filters.Add(Builders<ReservationEntity>.Filter.All("Assignments", typeFilter.Keys.ToArray()));
            }
        }

        if (status.HasValue)
        {
            filters.Add(Builders<ReservationEntity>.Filter.Eq("Status", new BsonInt32(status.Value)));
        }

        if (type.HasValue)
        {
            filters.Add(Builders<ReservationEntity>.Filter.Eq("Type", new BsonInt32(type.Value)));
        }

        //var combinedFilters = Builders<Core.Models.ReservationDbo>.Filter.And(filters);
        var combinedFilters = Builders<ReservationEntity>.Filter.And(filters);

        var result = await _reservationRepository.FilterAsync(combinedFilters);

        return result.Select(ReservationItem.FromReservationEntity).ToList();
    }

}
