using System.Net;
using Microservice.Appointments.IntegrationTests.Endpoints.Base;
using Microservice.Appointments.IntegrationTests.Helpers;
using Microservice.Appointments.IntegrationTests.MessageBus;
using Microservice.Appointments.IntegrationTests.Responses;
using RestSharp;
using Xunit;

namespace Microservice.Appointments.IntegrationTests.EventHandlers;

public class AppointmentNotificationHandlerTests : TestsBase
{
    private const int DefaultTimeout = 2000;

    [Fact]
    public async Task Given_Valid_Notification_Event_When_Handled_Then_Entity_Is_Updated()
    {
        var notificationEvent = new AppointmentNotificationEvent(
            "600-notification",
            600,
            "AppointmentNotificationEvent",
            "Updated Title",
            DateTime.Parse("2024-12-28T11:00:00Z"),
            DateTime.Parse("2024-12-28T12:00:00Z"),
            "Updated Description",
            2
        );

        await EventBus.PublishAsync(notificationEvent, EventHelper.GetEventName<AppointmentNotificationEvent>());
        await Task.Delay(DefaultTimeout);

        var getRequest = RequestHelper.GetRequest($"{Url.Appointments.GetById(600)}");
        var getResponse = await Client.ExecuteAsync<AppointmentResponse>(getRequest);

        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        Assert.NotNull(getResponse.Data);
        Assert.Equal(notificationEvent.Title, getResponse.Data.Title);
        Assert.Equal(notificationEvent.StartTime, getResponse.Data.StartTime);
        Assert.Equal(notificationEvent.EndTime, getResponse.Data.EndTime);
        Assert.Equal(notificationEvent.Description, getResponse.Data.Description);
        Assert.Equal(notificationEvent.Status, getResponse.Data.Status);
    }

    [Fact]
    public async Task Given_Invalid_Event_Type_When_Handled_Then_No_Changes_Are_Made()
    {
        var notificationEvent = new AppointmentNotificationEvent(
            "601-notification",
            601,
            "InvalidNotificationEventType",
            "Updated Title",
            DateTime.Parse("2024-12-29T15:00:00Z"),
            DateTime.Parse("2024-12-29T16:00:00Z"),
            "Updated Description",
            2
        );

        await EventBus.PublishAsync(notificationEvent, EventHelper.GetEventName<AppointmentNotificationEvent>());
        await Task.Delay(DefaultTimeout);

        var getRequest = RequestHelper.GetRequest($"{Url.Appointments.GetById(601)}");
        var getResponse = await Client.ExecuteAsync<AppointmentResponse>(getRequest);

        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        Assert.NotNull(getResponse.Data);
        Assert.Equal("Notification Invalid Event Type", getResponse.Data.Title);
    }

    [Fact]
    public async Task Given_Invalid_Data_When_Handled_Then_No_Changes_Are_Made()
    {
        var notificationEvent = new AppointmentNotificationEvent(
            "602-notification",
            602,
            "AppointmentNotificationEvent",
            "Invalid Update",
            DateTime.Parse("2024-12-30T13:00:00Z"),
            DateTime.Parse("2024-12-30T12:00:00Z"),
            "Description causing validation error",
            1
        );

        await EventBus.PublishAsync(notificationEvent, EventHelper.GetEventName<AppointmentNotificationEvent>());
        await Task.Delay(DefaultTimeout);

        var getRequest = RequestHelper.GetRequest($"{Url.Appointments.GetById(602)}");
        var getResponse = await Client.ExecuteAsync<AppointmentResponse>(getRequest);

        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        Assert.NotNull(getResponse.Data);
        Assert.Equal("Notification Non-updatable Appointment", getResponse.Data.Title);
    }
}
