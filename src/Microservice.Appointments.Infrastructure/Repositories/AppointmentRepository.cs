using Microservice.Appointments.Application.Repositories;
using Microservice.Appointments.Domain.Models;

namespace Microservice.Appointments.Infrastructure.Repositories
{
    public class AppointmentRepository : IAppointmentRepository
    {
        //TODO: Use Entity Mappers here when EF Core is implemented
        AppointmentDomain _mockedAppointmentDomain = new AppointmentDomain("Sample Title", DateTime.UtcNow, DateTime.UtcNow.AddHours(1), "Sample Description");

        public async Task<IEnumerable<AppointmentDomain>> GetAsync()
        {
            return await Task.FromResult<IEnumerable<AppointmentDomain>>(new List<AppointmentDomain>
            {
                _mockedAppointmentDomain
            });
        }

        public async Task<AppointmentDomain> GetAsync(int id)
        {
            return await Task.FromResult(_mockedAppointmentDomain);
        }

        public async Task<AppointmentDomain> AddAsync(AppointmentDomain appointmentDomain)
        {
            return await Task.FromResult(_mockedAppointmentDomain);
        }
    }
}