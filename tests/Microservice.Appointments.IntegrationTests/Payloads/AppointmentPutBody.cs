namespace Microservice.Appointments.IntegrationTests.Payloads;

public record AppointmentPutBody(string Title, DateTime StartTime, DateTime EndTime, string Description, int Status);