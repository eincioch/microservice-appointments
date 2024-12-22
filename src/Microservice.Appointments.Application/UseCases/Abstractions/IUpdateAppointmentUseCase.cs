using Microservice.Appointments.Application.Dtos.Appointments;
using Microservice.Appointments.Domain.Enums;

namespace Microservice.Appointments.Application.UseCases.Abstractions;

public interface IUpdateAppointmentUseCase
{
    Task<AppointmentDto> ExecuteAsync(int id, string title, DateTime startTime, DateTime endTime, string description, AppointmentStatus status);
}