using System.Net;
using Microservice.Appointments.IntegrationTests.Endpoints.Base;
using Microservice.Appointments.IntegrationTests.Helpers;
using Microservice.Appointments.IntegrationTests.MessageBus;
using Microservice.Appointments.IntegrationTests.Payloads;
using Microservice.Appointments.IntegrationTests.Responses;
using RestSharp;
using Xunit;

namespace Microservice.Appointments.IntegrationTests.Endpoints;

public class PatchAppointmentTests : TestsBase
{
    [Fact]
    public async Task Given_Valid_Request_When_Patch_Then_Returns_Ok()
    {
        // Arrange
        var patchPayload = new AppointmentPatchBody(2);
        var request = RequestHelper.PatchRequest($"{Url.Appointments.Patch(400)}", patchPayload);

        // Act
        var response = await Client.ExecuteAsync<AppointmentResponse>(request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(response.Data);
        Assert.Equal(2, response.Data.Status);
    }

    [Fact]
    public async Task Given_NonExistent_AppointmentId_When_Patch_Then_Returns_NotFound()
    {
        var patchPayload = new AppointmentPatchBody(2);
        var request = RequestHelper.PatchRequest($"{Url.Appointments.Patch(999)}", patchPayload);

        var response = await Client.ExecuteAsync<ProblemDetailsResponse>(request);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.NotNull(response.Data);
        Assert.Contains("not found", response.Data.Detail, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Given_Invalid_Status_When_Patch_Then_Returns_BadRequest()
    {
        var invalidPayload = new AppointmentPatchBody(99);
        var request = RequestHelper.PatchRequest($"{Url.Appointments.Patch(401)}", invalidPayload);

        var response = await Client.ExecuteAsync<ProblemDetailsResponse>(request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.NotNull(response.Data);
        Assert.Contains("Validation error occurred", response.Data.Detail, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Given_Same_Status_When_Patch_Then_NoChange()
    {
        var patchPayload = new AppointmentPatchBody(3);
        var request = RequestHelper.PatchRequest($"{Url.Appointments.Patch(402)}", patchPayload);

        var response = await Client.ExecuteAsync<AppointmentResponse>(request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(response.Data);
        Assert.Equal(3, response.Data.Status);
    }

    [Fact]
    public async Task Given_Valid_Request_When_Patch_Then_Event_Is_Published()
    {
        var eventBusHelper = new EventBusHelper(EventBus);
        await eventBusHelper.SubscribeToEventAsync<AppointmentChangedEvent>();

        var patchPayload = new AppointmentPatchBody(2);
        var request = RequestHelper.PatchRequest($"{Url.Appointments.Patch(400)}", patchPayload);

        var response = await Client.ExecuteAsync<AppointmentResponse>(request);
        var receivedEvent = await eventBusHelper.WaitForEventAsync<AppointmentChangedEvent>(
            x => x.AppointmentId == 400 && x.Status == patchPayload.Status
        );

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(response.Data);
        Assert.NotNull(receivedEvent);
        Assert.Equal(400, receivedEvent!.AppointmentId);
        Assert.Equal(patchPayload.Status, receivedEvent.Status);
    }
}