using Microservice.Appointments.Domain.Enums;

namespace Microservice.Appointments.Domain.Events;

public record AppointmentCreatedEvent(int AppointmentId, string Title, DateTime StartTime, DateTime EndTime, string Description, AppointmentStatus Status);