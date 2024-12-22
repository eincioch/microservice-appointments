using Microservice.Appointments.Domain.Models;

namespace Microservice.Appointments.Application.Repositories;

public interface IAppointmentRepository
{
    Task<IEnumerable<AppointmentDomain>> GetAsync();
    Task<AppointmentDomain> GetAsync(int id);
    Task<AppointmentDomain> AddAsync(AppointmentDomain appointmentDomain);
    Task<AppointmentDomain> UpdateAsync(AppointmentDomain appointmentDomain);
}