using Microservice.Appointments.Application.Dtos.Appointments;

namespace Microservice.Appointments.Application.UseCases.Abstractions;

public interface IGetAppointmentByIdUseCase
{
    Task<AppointmentDto> ExecuteAsync(int id);
}