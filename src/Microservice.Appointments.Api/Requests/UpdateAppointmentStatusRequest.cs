using Microservice.Appointments.Domain.Enums;

namespace Microservice.Appointments.Api.Requests;

public record UpdateAppointmentStatusRequest(AppointmentStatus Status);