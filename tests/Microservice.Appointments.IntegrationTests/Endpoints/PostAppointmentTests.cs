using System.Net;
using Microservice.Appointments.IntegrationTests.Endpoints.Base;
using Microservice.Appointments.IntegrationTests.Helpers;
using Microservice.Appointments.IntegrationTests.MessageBus;
using Microservice.Appointments.IntegrationTests.Payloads;
using Microservice.Appointments.IntegrationTests.Responses;
using RestSharp;
using Xunit;

namespace Microservice.Appointments.IntegrationTests.Endpoints;

public class PostAppointmentTests : TestsBase
{
    [Fact]
    public async Task Given_Valid_Request_When_Post_Then_Return_CreatedStatusOnly()
    {
        // Arrange
        var postPayload = new AppointmentPostBody(
            "New Appointment",
            DateTime.Parse("2024-12-28T10:00:00Z"),
            DateTime.Parse("2024-12-28T11:00:00Z"),
            "Test appointment creation"
        );

        var request = RequestHelper.PostRequest(Url.Appointments.Post, postPayload);

        // Act
        var response = await Client.ExecuteAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task Given_Valid_Request_When_Post_Then_Return_AppointmentResponse()
    {
        // Arrange
        var postPayload = new AppointmentPostBody(
            "Another Appointment",
            DateTime.Parse("2024-12-28T09:00:00Z"),
            DateTime.Parse("2024-12-28T09:30:00Z"),
            "Another test appointment"
        );

        var request = RequestHelper.PostRequest(Url.Appointments.Post, postPayload);

        // Act
        var response = await Client.ExecuteAsync<AppointmentResponse>(request);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(response.Data);
        Assert.True(response.Data.Id > 0);
        Assert.Equal(postPayload.Title, response.Data.Title);
        Assert.Equal(postPayload.StartTime, response.Data.StartTime);
        Assert.Equal(postPayload.EndTime, response.Data.EndTime);
        Assert.Equal(postPayload.Description, response.Data.Description);
    }

    [Fact]
    public async Task Given_Valid_Request_When_Post_Then_CanRetrieveWithGet()
    {
        // Arrange
        var postPayload = new AppointmentPostBody(
            "Meeting with Team",
            DateTime.Parse("2024-12-28T09:00:00Z"),
            DateTime.Parse("2024-12-28T09:15:00Z"),
            "Short meeting"
        );

        var postReq = RequestHelper.PostRequest(Url.Appointments.Post, postPayload);

        // Act
        var postRes = await Client.ExecuteAsync<AppointmentResponse>(postReq);

        // Assert
        Assert.Equal(HttpStatusCode.Created, postRes.StatusCode);
        Assert.NotNull(postRes.Data);

        // Act
        var getReq = RequestHelper.GetRequest($"{Url.Appointments.GetById(postRes.Data.Id)}");
        var getRes = await Client.ExecuteAsync<AppointmentResponse>(getReq);

        // Assert
        Assert.Equal(HttpStatusCode.OK, getRes.StatusCode);
        Assert.NotNull(getRes.Data);
        Assert.Equal(postRes.Data.Id, getRes.Data.Id);
        Assert.Equal(postRes.Data.Title, getRes.Data.Title);
        Assert.Equal(postRes.Data.StartTime, getRes.Data.StartTime);
        Assert.Equal(postRes.Data.EndTime, getRes.Data.EndTime);
        Assert.Equal(postRes.Data.Description, getRes.Data.Description);
    }

    [Theory]
    [InlineData("", "2024-12-28T10:00:00Z", "2024-12-28T11:00:00Z", "Desc")]
    [InlineData("    ", "2024-12-28T10:00:00Z", "2024-12-28T11:00:00Z", "Desc")]
    [InlineData("Valid Title", "2024-12-29T11:00:00Z", "2024-12-29T10:00:00Z", "Desc")]
    [InlineData("Valid Title", "2024-12-30T09:00:00Z", "2024-12-30T09:00:00Z", "Desc")]

    public async Task Given_Invalid_Request_When_Post_Then_Return_BadRequest(
        string title,
        string startTime,
        string endTime,
        string description)
    {
        // Arrange
        var postPayload = new AppointmentPostBody(
            title,
            DateTime.Parse(startTime),
            DateTime.Parse(endTime),
            description
        );

        var request = RequestHelper.PostRequest(Url.Appointments.Post, postPayload);

        // Act
        var response = await Client.ExecuteAsync<ProblemDetailsResponse>(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.NotNull(response.Data);
        Assert.NotNull(response.Data.Detail);
    }

    [Fact]
    public async Task Given_Valid_Request_When_Post_Then_Event_Is_Published()
    {
        // Arrange
        var eventBusHelper = new EventBusHelper(EventBus);
        await eventBusHelper.SubscribeToEventAsync<AppointmentCreatedEvent>();

        var payload = new AppointmentPostBody(
            "Integration Test Appointment",
            DateTime.Parse("2024-12-28T10:00:00Z"),
            DateTime.Parse("2024-12-28T11:00:00Z"),
            "Creating an appointment to test message bus event"
        );

        var request = RequestHelper.PostRequest(Url.Appointments.Post, payload);

        // Act
        var response = await Client.ExecuteAsync<AppointmentResponse>(request);
        var receivedEvent = await eventBusHelper.WaitForEventAsync<AppointmentCreatedEvent>(
x => x.Title == payload.Title && x.StartTime == payload.StartTime
        );

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(response.Data);
        Assert.NotNull(receivedEvent);

        Assert.Equal(response.Data.Id, receivedEvent!.AppointmentId);
        Assert.Equal(response.Data.Title, receivedEvent.Title);
        Assert.Equal(response.Data.StartTime, receivedEvent.StartTime);
        Assert.Equal(response.Data.EndTime, receivedEvent.EndTime);
        Assert.Equal(response.Data.Description, receivedEvent.Description);
    }
}