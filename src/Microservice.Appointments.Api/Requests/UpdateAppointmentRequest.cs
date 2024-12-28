using Microservice.Appointments.Domain.Enums;

namespace Microservice.Appointments.Api.Requests;

public record UpdateAppointmentRequest(
    string Title,
    DateTime StartTime,
    DateTime EndTime,
    string Description,
    AppointmentStatus Status
);