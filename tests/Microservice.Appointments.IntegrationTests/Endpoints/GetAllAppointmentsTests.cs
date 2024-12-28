using System.Net;
using Microservice.Appointments.IntegrationTests.Endpoints.Base;
using Microservice.Appointments.IntegrationTests.Helpers;
using Microservice.Appointments.IntegrationTests.Responses;
using RestSharp;
using Xunit;

namespace Microservice.Appointments.IntegrationTests.Endpoints;

public class GetAllAppointmentsTests : TestsBase
{
    [Fact]
    public async Task Given_Valid_Request_When_GetAll_Then_Return_Ok()
    {
        // Arrange
        var request = RequestHelper.GetRequest(Url.Appointments.GetAll);

        // Act
        var response = await Client.ExecuteAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Given_Valid_Request_When_GetAll_Then_Return_ExpectedData()
    {
        // Arrange
        var request = RequestHelper.GetRequest(Url.Appointments.GetAll);

        var expectedAppointments = new List<AppointmentResponse>
        {
            new(200, "Daily Standup", DateTime.Parse("2024-12-28T09:00:00"), DateTime.Parse("2024-12-28T09:15:00"), "Quick sync with the team to discuss daily tasks", 1),
            new(201, "Server Maintenance", DateTime.Parse("2024-12-28T23:00:00"), DateTime.Parse("2024-12-29T03:00:00"), "Scheduled maintenance for production server", 2),
            new(202, "Sprint Planning", DateTime.Parse("2024-12-29T10:00:00"), DateTime.Parse("2024-12-29T11:30:00"), "Plan tasks for the next sprint with the team", 1),
            new(203, "Deployment Window", DateTime.Parse("2024-12-30T01:00:00"), DateTime.Parse("2024-12-30T03:00:00"), "Deploying version 2.3 of the application", 1),
            new(204, "Retrospective Meeting", DateTime.Parse("2024-12-30T16:00:00"), DateTime.Parse("2024-12-30T17:00:00"), "Review the last sprint and discuss improvements", 3)
        };

        // Act
        var response = await Client.ExecuteAsync<List<AppointmentResponse>>(request);

        // Assert
        Assert.NotNull(response);
        Assert.NotNull(response.Data);

        foreach (var expected in expectedAppointments)
        {
            Assert.Contains(response.Data, actual =>
                actual.Id == expected.Id &&
                actual.Title == expected.Title &&
                actual.StartTime == expected.StartTime &&
                actual.EndTime == expected.EndTime &&
                actual.Description == expected.Description &&
                actual.Status == expected.Status);
        }
    }
}