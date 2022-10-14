using Microsoft.AspNetCore.Mvc;
using Scheduling.Application.Interfaces;
using Scheduling.Application.Models;
using Scheduling.Domain.Enums;
using Scheduling.Reservation.Models;
using Scheduling.Reservation.Utils;
using Scheduling.Domain.Models;

namespace Scheduling.Reservation.Controllers;

[ApiController]
[Route("[controller]")]
public class ReservationController : ControllerBase
{
    private readonly ILogger<ReservationController> _logger;
    private readonly IReservationService _reservationService;

    public ReservationController(ILogger<ReservationController> logger, IReservationService reservationService)
    {
        _reservationService = reservationService;
        _logger = logger;
    }

    [HttpPost]
    public async Task<ReservationResponse> Create(CreateReservationRequest request)
    {
        _logger.LogInformation($"Creating reservation for {request.ProductGroup}:{request.Account}");

        var reservation = new ReservationItem()
        {
            
        };
        
        var result = await _reservationService.CreateAsync(reservation);

        return new ReservationResponse();


    }

    [HttpGet]
    public async Task<List<ReservationResponse>> Get()
    {
        _logger.LogInformation($"Listing reservations");

        // Obtain these from token or whatever mechanism we will eb using for service to service comms
        var account = "fakeAccount";
        var productGroup = "fakeProductGroup";

        //return await _reservationService.GetAsync(productGroup, account);
        return new List<ReservationResponse>();
    }

    [HttpGet]
    [Route("{id}")]
    public async Task<ReservationResponse> GetById(string id)
    {
        _logger.LogInformation($"Listing Reservation {id}");
        //return await _reservationService.GetByIdAsync(id);
        return new ReservationResponse();
    }

    [HttpPost()]
    [Route("query")]
    public async Task<List<ReservationResponse>> Query(ReservationFilteredRequest request)
    {
        _logger.LogInformation($"Listing reservations");
        
        if (string.IsNullOrWhiteSpace(request?.Account))
        {
            throw new HttpRequestException("Account is required.");
        }
        
        if (string.IsNullOrWhiteSpace(request?.ProductGroup))
        {
            throw new HttpRequestException("ProductGroup is required.");
        }

        request.StartTime = request.StartTime ?? DateTime.UtcNow.Date;
        request.EndTime = request.EndTime ?? request.StartTime.Value.AddMonths(1).AddMilliseconds(-1);

        if (request.StartTime > request.EndTime)
        {
            throw new HttpRequestException("StartTime must be less than or equal to the EndTime.");
        }

        var filters = GetFiltersFromQueryRequest(request);

        var result = await _reservationService.FilterAsync(
            request.ProductGroup,
            request.Account,
            request.StartTime,
            request.EndTime,
            filters,
            request.Status,
            request.Type
        );

        return result.Select(x => new ReservationResponse
        {
            Id = x.Id,
            Duration = x.Duration,
            Spots = x.Spots,
            Status = x.Status,
            Type = x.Type,
            AssignmentDetails = x.AssignmentDetails ?? new List<AssignmentSummary>(),
            StartTime = x.StartTime,
            EndTime = x.StartTime.AddMinutes(x.Duration)
        }).ToList();
    }

    [HttpDelete]
    public void Delete(string productGroup, string id)
    {
        _logger.LogInformation($"Deleting reservation for {productGroup}:{id}");
        _reservationService.RemoveAsync(id);
    }

    [HttpPut]
    public async Task<ReservationResponse> Update(CreateReservationRequest reservationDbo)
    {
        _logger.LogInformation($"Updating reservation for {reservationDbo.ProductGroup}:{reservationDbo.Account}");
        //return await _reservationService.UpdateAsync(reservationDbo);
        // TODO: Fix Me!
        throw new NotImplementedException();
    }
    

    [HttpPut]
    [Route("{id}/status/{status}")]
    public async Task<ReservationResponse> UpdateStatus(string id, int status)
    {
        var reservation = await _reservationService.GetByIdAsync(id);

        if (reservation == null)
        {
            throw new ArgumentException($"No record found with id '{id}' so no update can occur.");
        }

        _logger.LogInformation($"Updating reservation for {reservation?.ProductGroup}:{reservation?.Account}");
        var result = await _reservationService.UpdateStatusAsync(id, status);

        return new ReservationResponse
        {
            Id = result.Id,
            Duration = result.Duration,
            Spots = result.Spots,
            Status = result.Status,
            Type = result.Type,
            AssignmentDetails = result.AssignmentDetails ?? new List<AssignmentSummary>(),
            StartTime = result.StartTime,
            EndTime = result.StartTime.AddMinutes(result.Duration)
        };
    }

    #region Reservation Types

    [HttpGet]
    [Route("types/assignment")]
    public List<EnumDto> GetAssignmentTypes()
    {
        _logger.LogInformation($"Listing Assignment Types");

        return Enum<AssignmentType>.GetAllValuesAsIEnumerable().Select(d => new EnumDto(d)).ToList();
    }

    [HttpGet]
    [Route("types/reservation")]
    public List<EnumDto> GetReservationTypes()
    {
        _logger.LogInformation($"Listing Assignment Types");

        return Enum<ReservationType>.GetAllValuesAsIEnumerable().Select(d => new EnumDto(d)).ToList();
    }

    [HttpGet]
    [Route("types/status")]
    public List<EnumDto> GetStatusTypes()
    {
        _logger.LogInformation($"Listing Reservation Status Types");

        return Enum<ReservationStatus>.GetAllValuesAsIEnumerable().Select(d => new EnumDto(d)).ToList();
    }

    #endregion

    #region Helpers

    public static List<ReservationTypeFilter> GetFiltersFromQueryRequest(ReservationFilteredRequest request)
    {
        List<ReservationTypeFilter> filters = new List<ReservationTypeFilter>();

        if (request.Customers?.Count > 0)
        {
            filters.Add(GetFilterForAssignmentTypePrefix(request.Customers, AssignmentType.Customer));
        }

        if (request.Employees?.Count > 0)
        {
            filters.Add(GetFilterForAssignmentTypePrefix(request.Employees, AssignmentType.Employee));
        }

        if (request.Locations?.Count > 0)
        {
            filters.Add(GetFilterForAssignmentTypePrefix(request.Locations, AssignmentType.Location));
        }

        if (request.Resources?.Count > 0)
        {
            filters.Add(GetFilterForAssignmentTypePrefix(request.Resources, AssignmentType.Resource));
        }

        if (request.Rooms?.Count > 0)
        {
            filters.Add(GetFilterForAssignmentTypePrefix(request.Rooms, AssignmentType.Room));
        }

        if (request.Routes?.Count > 0)
        {
            filters.Add(GetFilterForAssignmentTypePrefix(request.Routes, AssignmentType.Route));
        }

        if (request.Services?.Count > 0)
        {
            filters.Add(GetFilterForAssignmentTypePrefix(request.Services, AssignmentType.Service));
        }

        return filters;
    }

    public static ReservationTypeFilter GetFilterForAssignmentTypePrefix(
            List<string> ids,
            AssignmentType type
        )
    {
        if (ids?.Count <= 0)
        {
            throw new ArgumentNullException("ids");
        }

        var prefixedKeys = ids.Select(id => (int)type + "-" + id).ToList();
        return new ReservationTypeFilter((int)type, prefixedKeys);
    }

    #endregion

}
