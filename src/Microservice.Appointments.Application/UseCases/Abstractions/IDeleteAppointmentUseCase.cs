namespace Microservice.Appointments.Application.UseCases.Abstractions;

public interface IDeleteAppointmentUseCase
{
    Task ExecuteAsync(int id);
}