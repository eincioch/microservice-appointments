namespace Microservice.Appointments.IntegrationTests.Payloads;

public record AppointmentPostBody(string Title, DateTime StartTime, DateTime EndTime, string Description);