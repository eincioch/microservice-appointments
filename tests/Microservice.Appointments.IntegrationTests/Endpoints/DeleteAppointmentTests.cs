using System.Net;
using Microservice.Appointments.IntegrationTests.Endpoints.Base;
using Microservice.Appointments.IntegrationTests.Helpers;
using Microservice.Appointments.IntegrationTests.MessageBus;
using Microservice.Appointments.IntegrationTests.Responses;
using RestSharp;
using Xunit;

namespace Microservice.Appointments.IntegrationTests.Endpoints;

public class DeleteAppointmentTests : TestsBase
{
    [Fact]
    public async Task Given_Valid_Request_When_Delete_Then_Returns_NoContent()
    {
        // Arrange
        var request = RequestHelper.DeleteRequest($"{Url.Appointments.Delete(500)}");

        // Act
        var response = await Client.ExecuteAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task Given_NonExistent_AppointmentId_When_Delete_Then_Returns_NotFound()
    {
        // Arrange
        var request = RequestHelper.DeleteRequest($"{Url.Appointments.Delete(999)}");

        // Act
        var response = await Client.ExecuteAsync<ProblemDetailsResponse>(request);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.NotNull(response.Data);
        Assert.Contains("not found", response.Data.Detail, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Given_Valid_Request_When_Delete_Then_Event_Is_Published()
    {
        // Arrange
        var eventBusHelper = new EventBusHelper(EventBus);
        await eventBusHelper.SubscribeToEventAsync<AppointmentDeletedEvent>();

        var expectedDeletedAppointment = new AppointmentDeletedEvent(
            501,
            "Delete Backlog Refinement",
            TimeZoneHelper.ToUtc(DateTime.Parse("2024-12-28T11:00:00")),
            TimeZoneHelper.ToUtc(DateTime.Parse("2024-12-28T11:30:00")),
            "Morning sync meeting",
            1
        );

        var request = RequestHelper.DeleteRequest($"{Url.Appointments.Delete(expectedDeletedAppointment.AppointmentId)}");

        // Act
        var response = await Client.ExecuteAsync(request);
        var receivedEvent = await eventBusHelper.WaitForEventAsync<AppointmentDeletedEvent>(
            x => x.AppointmentId == expectedDeletedAppointment.AppointmentId
        );

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        Assert.NotNull(receivedEvent);
        Assert.Equal(expectedDeletedAppointment.AppointmentId, receivedEvent!.AppointmentId);
        Assert.Equal(expectedDeletedAppointment.Title, receivedEvent.Title);
        Assert.Equal(expectedDeletedAppointment.StartTime.ToUniversalTime(), receivedEvent.StartTime.ToUniversalTime());
        Assert.Equal(expectedDeletedAppointment.EndTime.ToUniversalTime(), receivedEvent.EndTime.ToUniversalTime());
        Assert.Equal(expectedDeletedAppointment.Description, receivedEvent.Description);
        Assert.Equal(expectedDeletedAppointment.Status, receivedEvent.Status);
    }
}