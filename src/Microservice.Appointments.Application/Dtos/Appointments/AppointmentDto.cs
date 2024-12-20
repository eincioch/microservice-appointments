using Microservice.Appointments.Domain.Enums;

namespace Microservice.Appointments.Application.Dtos.Appointments;

public record AppointmentDto(int Id, string Title, DateTime StartTime, DateTime EndTime, string Description, AppointmentStatus Status);