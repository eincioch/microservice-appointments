using Microservice.Appointments.Application.Dtos.Appointments;

namespace Microservice.Appointments.Application.UseCases.Abstractions;

public interface ICreateAppointmentUseCase
{
    Task<AppointmentDto> ExecuteAsync(string title, DateTime startTime, DateTime endTime, string description);
}