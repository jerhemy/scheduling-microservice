using Scheduling.Reservation.Controllers;
using Moq;
using Microsoft.Extensions.Logging;
using Scheduling.Application.Interfaces;
using Scheduling.Domain.Enums;
using Scheduling.Domain.Models;

namespace Scheduling.Reservation.Tests;

public class ReservationControllerTests
{
    private Mock<IReservationService> reservationServiceMock;
    ReservationController controller;
    Models.ReservationFilteredRequest request;
    public ReservationControllerTests()
    {
        var reservationService = Mock.Of<IReservationService>();
        reservationServiceMock = Mock.Get(reservationService);
        controller = new ReservationController(
                            Mock.Of<ILogger<ReservationController>>(),
                            reservationService
                            );
        request = new Models.ReservationFilteredRequest()
        {
            Account = "1",
            ProductGroup = "AF"
        };
    }

    [Fact]
    public void GetFilterForAssignmentTypePrefix_WhenIDsNullThrows()
    {
        Assert.Throws<ArgumentNullException>(
            () => ReservationController.GetFilterForAssignmentTypePrefix(
                null,
                AssignmentType.Customer
            )
        );
    }

    [Fact]
    public void GetFilterForAssignmentTypePrefix_WhenIDsEmptyThrows()
    {
        Assert.Throws<ArgumentNullException>(
            () => ReservationController.GetFilterForAssignmentTypePrefix(
                new List<string>(),
                AssignmentType.Customer
            )
        );
    }

    [Fact]
    public void GetFilterForAssignmentTypePrefix_WhenIDsHasIDsExpectFilter()
    {
        var filter = ReservationController.GetFilterForAssignmentTypePrefix(
                        new List<string>() { "1" },
                        AssignmentType.Customer
                    );
        Assert.NotNull(
            filter
        );
        Assert.Single(filter.Keys);
        Assert.Equal("0-1", filter.Keys[0]);
        Assert.Equal(AssignmentType.Customer, (AssignmentType)filter.Type);
    }

    [Fact]
    public void GetFiltersFromQueryRequest_WhenNoIDsExpect0Filters()
    {
        Assert.Empty(
            ReservationController.GetFiltersFromQueryRequest(
                new Models.ReservationFilteredRequest()
            )
        );
    }

    [Fact]
    public void GetFiltersFromQueryRequest_WhenEmptyIDsExpect0Filters()
    {
        var filters = ReservationController.GetFiltersFromQueryRequest(
                        new Models.ReservationFilteredRequest()
                        {
                            Customers = new List<string>(),
                            Employees = new List<string>(),
                            Locations = new List<string>(),
                            Resources = new List<string>(),
                            Rooms = new List<string>(),
                            Routes = new List<string>(),
                            Services = new List<string>()
                        }
                    );
        Assert.Empty(
            filters
        );
    }

    [Fact]
    public void GetFiltersFromQueryRequest_WhenHasServiceIDsExpect1Filter()
    {
        var filters = ReservationController.GetFiltersFromQueryRequest(
                        new Models.ReservationFilteredRequest()
                        {
                            Services = new List<string>() { "1" }
                        }
                    );

        Assert.Single(filters);
        const AssignmentType type = AssignmentType.Service;
        Assert.Equal(type, (AssignmentType)filters[0].Type);
        Assert.Single(filters[0].Keys);
        Assert.Equal((int)type + "-1", filters[0].Keys[0]);
    }

    [Fact]
    public void GetFiltersFromQueryRequest_WhenHasCustomerIDsExpect1Filter()
    {
        var filters = ReservationController.GetFiltersFromQueryRequest(
                        new Models.ReservationFilteredRequest()
                        {
                            Customers = new List<string>() { "1" }
                        }
                    );

        Assert.Single(filters);
        const AssignmentType type = AssignmentType.Customer;
        Assert.Equal(type, (AssignmentType)filters[0].Type);
        Assert.Single(filters[0].Keys);
        Assert.Equal((int)type + "-1", filters[0].Keys[0]);
    }

    [Fact]
    public void GetFiltersFromQueryRequest_WhenHasEmployeesIDsExpect1Filter()
    {
        var filters = ReservationController.GetFiltersFromQueryRequest(
                        new Models.ReservationFilteredRequest()
                        {
                            Employees = new List<string>() { "1" }
                        }
                    );

        Assert.Single(filters);
        const AssignmentType type = AssignmentType.Employee;
        Assert.Equal(type, (AssignmentType)filters[0].Type);
        Assert.Single(filters[0].Keys);
        Assert.Equal((int)type + "-1", filters[0].Keys[0]);
    }

    [Fact]
    public void GetFiltersFromQueryRequest_WhenHasLocationsIDsExpect1Filter()
    {
        var filters = ReservationController.GetFiltersFromQueryRequest(
                        new Models.ReservationFilteredRequest()
                        {
                            Locations = new List<string>() { "1" }
                        }
                    );

        Assert.Single(filters);
        const AssignmentType type = AssignmentType.Location;
        Assert.Equal(type, (AssignmentType)filters[0].Type);
        Assert.Single(filters[0].Keys);
        Assert.Equal((int)type + "-1", filters[0].Keys[0]);
    }

    [Fact]
    public void GetFiltersFromQueryRequest_WhenHasResourcesIDsExpect1Filter()
    {
        var filters = ReservationController.GetFiltersFromQueryRequest(
                        new Models.ReservationFilteredRequest()
                        {
                            Resources = new List<string>() { "1" }
                        }
                    );

        Assert.Single(filters);
        const AssignmentType type = AssignmentType.Resource;
        Assert.Equal(type, (AssignmentType)filters[0].Type);
        Assert.Single(filters[0].Keys);
        Assert.Equal((int)type + "-1", filters[0].Keys[0]);
    }

    [Fact]
    public void GetFiltersFromQueryRequest_WhenHasRoomsIDsExpect1Filter()
    {
        var filters = ReservationController.GetFiltersFromQueryRequest(
                        new Models.ReservationFilteredRequest()
                        {
                            Rooms = new List<string>() { "1" }
                        }
                    );

        Assert.Single(filters);
        const AssignmentType type = AssignmentType.Room;
        Assert.Equal(type, (AssignmentType)filters[0].Type);
        Assert.Single(filters[0].Keys);
        Assert.Equal((int)type + "-1", filters[0].Keys[0]);
    }

    [Fact]
    public void GetFiltersFromQueryRequest_WhenHasRoutesIDsExpect1Filter()
    {
        var filters = ReservationController.GetFiltersFromQueryRequest(
                        new Models.ReservationFilteredRequest()
                        {
                            Routes = new List<string>() { "1" }
                        }
                    );

        Assert.Single(filters);
        const AssignmentType type = AssignmentType.Route;
        Assert.Equal(type, (AssignmentType)filters[0].Type);
        Assert.Single(filters[0].Keys);
        Assert.Equal((int)type + "-1", filters[0].Keys[0]);
    }

    [Fact]
    public void Query_WhenNullRequestExpectThrowsException()
    {
        Assert.ThrowsAsync<HttpRequestException>(
           async () => await controller.Query(null)
        );
    }

    [Fact]
    public void Query_WhenNoAccountExpectThrowsException()
    {
        request.Account = null;
        Assert.ThrowsAsync<HttpRequestException>(
           async () => await controller.Query(request)
        );
    }

    [Fact]
    public void Query_WhenNoProductGroupExpectThrowsException()
    {
        request.ProductGroup = null;
        Assert.ThrowsAsync<HttpRequestException>(
           async () => await controller.Query(request)
        );
    }

    [Fact]
    public async void Query_WhenHasRequestExpectToNotThrowException()
    {
        reservationServiceMock.Setup(rs => rs.FilterAsync(
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<DateTimeOffset?>(),
            It.IsAny<DateTimeOffset?>(),
            It.IsAny<List<ReservationTypeFilter>>(),
            It.IsAny<int?>(),
            It.IsAny<int?>()
            )).Returns(Task.FromResult(new List<ReservationItem>()));
        var response = await controller.Query(request);
        Assert.NotNull(response);
        reservationServiceMock.Verify(rs => rs.FilterAsync(
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<DateTimeOffset?>(),
            It.IsAny<DateTimeOffset?>(),
            It.IsAny<List<ReservationTypeFilter>>(),
            It.IsAny<int?>(),
            It.IsAny<int?>()
            ), Times.Once);
    }
}
