namespace Microservice.Appointments.IntegrationTests.Payloads;

public record AppointmentCreateBody(string Title, DateTime StartTime, DateTime EndTime, string Description);