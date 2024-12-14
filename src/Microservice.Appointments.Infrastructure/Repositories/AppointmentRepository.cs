using Microservice.Appointments.Application.Repositories;
using Microservice.Appointments.Domain.Models;

namespace Microservice.Appointments.Infrastructure.Repositories
{
    public class AppointmentRepository : IAppointmentRepository
    {
        public Task<IEnumerable<Appointment>> GetAsync()
        {
            //TODO: Get Rid of this hardcoded data
            return Task.FromResult<IEnumerable<Appointment>>(new List<Appointment>
            {
                new Appointment("Sample Title", DateTime.UtcNow, DateTime.UtcNow.AddHours(1), "Sample Description")
            });
        }
    }
}