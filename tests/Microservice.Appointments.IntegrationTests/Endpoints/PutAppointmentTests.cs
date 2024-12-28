using System.Net;
using Microservice.Appointments.IntegrationTests.Endpoints.Base;
using Microservice.Appointments.IntegrationTests.Helpers;
using Microservice.Appointments.IntegrationTests.MessageBus;
using Microservice.Appointments.IntegrationTests.Payloads;
using Microservice.Appointments.IntegrationTests.Responses;
using RestSharp;
using Xunit;

namespace Microservice.Appointments.IntegrationTests.Endpoints;

public class PutAppointmentTests : TestsBase
{
    [Fact]
    public async Task Given_Valid_Request_When_Put_Then_Returns_Ok()
    {
        // Arrange
        var updatePayload = new AppointmentPutBody(
            "Updated Title",
            DateTime.Parse("2024-12-28T10:30:00Z"),
            DateTime.Parse("2024-12-28T11:30:00Z"),
            "Updated description",
            2
        );

        var request = RequestHelper.PutRequest($"{Url.Appointments.Put(300)}", updatePayload);

        // Act
        var response = await Client.ExecuteAsync<AppointmentResponse>(request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(response.Data);
        Assert.Equal(updatePayload.Title, response.Data.Title);
        Assert.Equal(updatePayload.Description, response.Data.Description);
        Assert.Equal(updatePayload.StartTime, response.Data.StartTime);
        Assert.Equal(updatePayload.EndTime, response.Data.EndTime);
        Assert.Equal(updatePayload.Status, response.Data.Status);
    }

    [Fact]
    public async Task Given_NonExistent_AppointmentId_When_Put_Then_Returns_NotFound()
    {
        // Arrange
        var updatePayload = new AppointmentPutBody(
            "Title for Non-Existent ID",
            DateTime.Parse("2024-12-28T13:00:00Z"),
            DateTime.Parse("2024-12-28T14:00:00Z"),
            "Description for Non-Existent ID",
            2
        );

        var request = RequestHelper.PutRequest($"{Url.Appointments.Put(999)}", updatePayload);

        // Act
        var response = await Client.ExecuteAsync<ProblemDetailsResponse>(request);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.NotNull(response.Data);
        Assert.Contains("was not found.", response.Data.Detail, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Given_Invalid_Status_When_Put_Then_Returns_BadRequest()
    {
        // Arrange
        var invalidPayload = new AppointmentPutBody(
            "Invalid Status Update",
            DateTime.Parse("2024-12-28T10:30:00Z"),
            DateTime.Parse("2024-12-28T11:30:00Z"),
            "Trying to set an invalid status",
            99
        );

        var request = RequestHelper.PutRequest($"{Url.Appointments.Put(301)}", invalidPayload);

        // Act
        var response = await Client.ExecuteAsync<ProblemDetailsResponse>(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.NotNull(response.Data);
        Assert.Contains("Validation error", response.Data.Detail, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Given_Valid_Request_When_Put_Then_Event_Is_Published()
    {
        // Arrange
        var eventBusHelper = new EventBusHelper(EventBus);
        await eventBusHelper.SubscribeToEventAsync<AppointmentChangedEvent>();

        var updatePayload = new AppointmentPutBody(
            "Updated for Event",
            DateTime.Parse("2024-12-28T15:00:00Z"),
            DateTime.Parse("2024-12-28T16:00:00Z"),
            "Updated description to test event",
            2
        );

        var request = RequestHelper.PutRequest($"{Url.Appointments.Put(300)}", updatePayload);

        // Act
        var response = await Client.ExecuteAsync<AppointmentResponse>(request);
        var receivedEvent = await eventBusHelper.WaitForEventAsync<AppointmentChangedEvent>(
            x => x.Title == updatePayload.Title && x.Status == updatePayload.Status
        );

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(response.Data);
        Assert.NotNull(receivedEvent);
        Assert.Equal(updatePayload.Title, receivedEvent.Title);
        Assert.Equal(updatePayload.Status, receivedEvent.Status);
    }
}