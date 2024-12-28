using System.Net;
using Microservice.Appointments.IntegrationTests.Endpoints.Base;
using Microservice.Appointments.IntegrationTests.Helpers;
using Microservice.Appointments.IntegrationTests.MessageBus;
using Microservice.Appointments.IntegrationTests.Payloads;
using Microservice.Appointments.IntegrationTests.Responses;
using RestSharp;
using Xunit;

namespace Microservice.Appointments.IntegrationTests.Endpoints;

public class CreateAppointmentTests : TestsBase
{
    [Fact]
    public async Task Given_Valid_Request_When_Create_Then_Return_CreatedStatusOnly()
    {
        // Arrange
        var createRequest = new AppointmentCreateBody(
            "New Appointment",
            DateTime.Parse("2024-12-28T10:00:00Z"),
            DateTime.Parse("2024-12-28T11:00:00Z"),
            "Test appointment creation"
        );

        var request = RequestHelper.PostRequest(Url.Appointments.Create, createRequest);

        // Act
        var response = await Client.ExecuteAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task Given_Valid_Request_When_Create_Then_Return_AppointmentResponse()
    {
        // Arrange
        var createRequest = new AppointmentCreateBody(
            "Another Appointment",
            DateTime.Parse("2024-12-28T09:00:00Z"),
            DateTime.Parse("2024-12-28T09:30:00Z"),
            "Another test appointment"
        );

        var request = RequestHelper.PostRequest(Url.Appointments.Create, createRequest);

        // Act
        var response = await Client.ExecuteAsync<AppointmentResponse>(request);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(response.Data);
        Assert.True(response.Data.Id > 0);
        Assert.Equal(createRequest.Title, response.Data.Title);
        Assert.Equal(createRequest.StartTime, response.Data.StartTime);
        Assert.Equal(createRequest.EndTime, response.Data.EndTime);
        Assert.Equal(createRequest.Description, response.Data.Description);
    }

    [Fact]
    public async Task Given_Valid_Request_When_Create_Then_CanRetrieveWithGet()
    {
        // Arrange
        var createRequest = new AppointmentCreateBody(
            "Meeting with Team",
            DateTime.Parse("2024-12-28T09:00:00Z"),
            DateTime.Parse("2024-12-28T09:15:00Z"),
            "Short meeting"
        );

        var createReq = RequestHelper.PostRequest(Url.Appointments.Create, createRequest);

        // Act
        var createRes = await Client.ExecuteAsync<AppointmentResponse>(createReq);

        // Assert
        Assert.Equal(HttpStatusCode.Created, createRes.StatusCode);
        Assert.NotNull(createRes.Data);

        // Act
        var getReq = RequestHelper.GetRequest($"{Url.Appointments.GetById(createRes.Data.Id)}");
        var getRes = await Client.ExecuteAsync<AppointmentResponse>(getReq);

        // Assert
        Assert.Equal(HttpStatusCode.OK, getRes.StatusCode);
        Assert.NotNull(getRes.Data);
        Assert.Equal(createRes.Data.Id, getRes.Data.Id);
        Assert.Equal(createRes.Data.Title, getRes.Data.Title);
        Assert.Equal(createRes.Data.StartTime, getRes.Data.StartTime);
        Assert.Equal(createRes.Data.EndTime, getRes.Data.EndTime);
        Assert.Equal(createRes.Data.Description, getRes.Data.Description);
    }

    [Theory]
    [InlineData("", "2024-12-28T10:00:00Z", "2024-12-28T11:00:00Z", "Desc")]
    [InlineData("    ", "2024-12-28T10:00:00Z", "2024-12-28T11:00:00Z", "Desc")]
    [InlineData("Valid Title", "2024-12-29T11:00:00Z", "2024-12-29T10:00:00Z", "Desc")]
    [InlineData("Valid Title", "2024-12-30T09:00:00Z", "2024-12-30T09:00:00Z", "Desc")]

    public async Task Given_Invalid_Request_When_Create_Then_Return_BadRequest(
        string title,
        string startTime,
        string endTime,
        string description)
    {
        // Arrange
        var createRequest = new AppointmentCreateBody(
            title,
            DateTime.Parse(startTime),
            DateTime.Parse(endTime),
            description
        );

        var request = RequestHelper.PostRequest(Url.Appointments.Create, createRequest);

        // Act
        var response = await Client.ExecuteAsync<ProblemDetailsResponse>(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.NotNull(response.Data);
        Assert.NotNull(response.Data.Detail);
    }

    [Fact]
    public async Task Given_Valid_Request_When_Create_Then_Event_Is_Published()
    {
        // Arrange
        var eventBusHelper = new EventBusHelper(EventBus);
        await eventBusHelper.SubscribeToEventAsync<AppointmentCreatedEvent>();

        var createRequest = new AppointmentCreateBody(
            "Integration Test Appointment",
            DateTime.Parse("2024-12-28T10:00:00Z"),
            DateTime.Parse("2024-12-28T11:00:00Z"),
            "Creating an appointment to test message bus event"
        );

        var postRequest = RequestHelper.PostRequest(Url.Appointments.Create, createRequest);

        // Act
        var postResponse = await Client.ExecuteAsync<AppointmentResponse>(postRequest);
        var receivedEvent = await eventBusHelper.WaitForEventAsync<AppointmentCreatedEvent>(
x => x.Title == createRequest.Title && x.StartTime == createRequest.StartTime
        );

        // Assert
        Assert.Equal(HttpStatusCode.Created, postResponse.StatusCode);
        Assert.NotNull(postResponse.Data);
        Assert.NotNull(receivedEvent);

        Assert.Equal(postResponse.Data.Id, receivedEvent!.AppointmentId);
        Assert.Equal(postResponse.Data.Title, receivedEvent.Title);
        Assert.Equal(postResponse.Data.StartTime, receivedEvent.StartTime);
        Assert.Equal(postResponse.Data.EndTime, receivedEvent.EndTime);
        Assert.Equal(postResponse.Data.Description, receivedEvent.Description);
    }
}