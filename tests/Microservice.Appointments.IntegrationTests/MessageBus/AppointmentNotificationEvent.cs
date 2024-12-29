namespace Microservice.Appointments.IntegrationTests.MessageBus;

public record AppointmentNotificationEvent(string NotificationId, int AppointmentId, string Type, string Title, DateTime StartTime, DateTime EndTime, string Description, int Status);