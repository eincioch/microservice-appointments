namespace Microservice.Appointments.Domain.Events
{
    public record AppointmentNotificationEvent(string NotificationId, int AppointmentId, string Type, string Status, DateTime SentAt);
}