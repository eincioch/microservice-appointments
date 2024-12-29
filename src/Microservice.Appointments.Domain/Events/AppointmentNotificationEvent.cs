using Microservice.Appointments.Domain.Enums;

namespace Microservice.Appointments.Domain.Events
{
    public record AppointmentNotificationEvent(string NotificationId, int AppointmentId, string Type, string Title, DateTime StartTime, DateTime EndTime, string Description, AppointmentStatus Status);
}