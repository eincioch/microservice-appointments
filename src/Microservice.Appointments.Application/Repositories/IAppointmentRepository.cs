using Microservice.Appointments.Domain.Models;

namespace Microservice.Appointments.Application.Repositories;

public interface IAppointmentRepository
{
    Task<IEnumerable<Appointment>> GetAsync();
    Task<Appointment> GetAsync(int id);
}