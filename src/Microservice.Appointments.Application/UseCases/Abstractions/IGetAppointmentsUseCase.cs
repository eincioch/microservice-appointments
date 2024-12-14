using Microservice.Appointments.Application.Dtos.Appointments;

namespace Microservice.Appointments.Application.UseCases.Abstractions;

public interface IGetAppointmentsUseCase
{
    Task<IEnumerable<AppointmentDto>> ExecuteAsync();
}