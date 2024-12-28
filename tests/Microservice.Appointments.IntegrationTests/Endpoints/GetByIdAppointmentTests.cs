using System.Net;
using Microservice.Appointments.IntegrationTests.Endpoints.Base;
using Microservice.Appointments.IntegrationTests.Helpers;
using Microservice.Appointments.IntegrationTests.Responses;
using RestSharp;
using Xunit;

namespace Microservice.Appointments.IntegrationTests.Endpoints;

public class GetByIdAppointmentTests : TestsBase
{
    [Fact]
    public async Task Given_Valid_Request_When_GetById_Then_Return_Ok()
    {
        // Arrange
        var validId = 200;
        var request = RequestHelper.GetRequest($"{Url.Appointments.GetById(validId)}");

        // Act
        var response = await Client.ExecuteAsync<AppointmentResponse>(request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(response.Data);
        Assert.Equal(validId, response.Data.Id);
    }

    [Fact]
    public async Task Given_Valid_Request_When_GetById_Then_Return_ExpectedData()
    {
        // Arrange
        var validId = 200;
        var request = RequestHelper.GetRequest($"{Url.Appointments.GetById(validId)}");

        var expectedAppointment = new AppointmentResponse(
            200,
            "Daily Standup",
            TimeZoneHelper.ToUtc(DateTime.Parse("2024-12-28T09:00:00")),
            TimeZoneHelper.ToUtc(DateTime.Parse("2024-12-28T09:15:00")),
            "Quick sync with the team to discuss daily tasks",
            1
        );
        // Act
        var response = await Client.ExecuteAsync<AppointmentResponse>(request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(response.Data);
        Assert.Equal(expectedAppointment.Id, response.Data.Id);
        Assert.Equal(expectedAppointment.Title, response.Data.Title);
        Assert.Equal(expectedAppointment.StartTime.ToUniversalTime(), response.Data.StartTime.ToUniversalTime());
        Assert.Equal(expectedAppointment.EndTime.ToUniversalTime(), response.Data.EndTime.ToUniversalTime());
        Assert.Equal(expectedAppointment.Description, response.Data.Description);
        Assert.Equal(expectedAppointment.Status, response.Data.Status);
    }


    [Fact]
    public async Task Given_Invalid_Request_When_GetById_Then_Return_BadRequest()
    {
        // Arrange
        var invalidId = -1;
        var request = RequestHelper.GetRequest($"{Url.Appointments.GetById(invalidId)}");

        // Act
        var response = await Client.ExecuteAsync<ProblemDetailsResponse>(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.NotNull(response.Data);
        Assert.Equal("Bad Request", response.Data.Title);
        Assert.Equal(400, response.Data.Status);
        Assert.NotNull(response.Data.Detail);
        Assert.Contains($"id '{invalidId}' is invalid", response.Data.Detail);
        Assert.Equal($"/Appointments/{invalidId}", response.Data.Instance);
    }

    [Fact]
    public async Task Given_Nonexistent_Id_When_GetById_Then_Return_NotFound()
    {
        // Arrange
        var nonexistentId = 999;
        var request = RequestHelper.GetRequest($"{Url.Appointments.GetById(nonexistentId)}");

        // Act
        var response = await Client.ExecuteAsync<ProblemDetailsResponse>(request);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.NotNull(response.Data);
        Assert.Equal("Not Found", response.Data.Title);
        Assert.Equal(404, response.Data.Status);
        Assert.NotNull(response.Data.Detail);
        Assert.Contains($"id '{nonexistentId}' was not found", response.Data.Detail);
        Assert.Equal($"/Appointments/{nonexistentId}", response.Data.Instance);
    }
}