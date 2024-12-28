namespace Microservice.Appointments.IntegrationTests.MessageBus;

public record AppointmentChangedEvent(int AppointmentId, string Title, DateTime StartTime, DateTime EndTime, string Description, int Status);