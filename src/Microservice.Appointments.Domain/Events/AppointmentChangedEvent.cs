using Microservice.Appointments.Domain.Enums;

namespace Microservice.Appointments.Domain.Events;

public record AppointmentChangedEvent(int AppointmentId, string Title, DateTime StartTime, DateTime EndTime, string Description, AppointmentStatus Status);