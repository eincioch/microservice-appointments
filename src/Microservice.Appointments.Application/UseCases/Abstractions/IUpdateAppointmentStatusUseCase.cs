using Microservice.Appointments.Application.Dtos.Appointments;
using Microservice.Appointments.Domain.Enums;

namespace Microservice.Appointments.Application.UseCases.Abstractions;

public interface IUpdateAppointmentStatusUseCase
{
    Task<AppointmentDto> ExecuteAsync(int id, AppointmentStatus status);
}