using Microservice.Appointments.Application.Repositories;
using Microservice.Appointments.Domain.Models;

namespace Microservice.Appointments.Infrastructure.Repositories
{
    public class AppointmentRepository : IAppointmentRepository
    {
        //TODO: Get Rid of this hardcoded data
        Appointment mockedAppointment = new Appointment("Sample Title", DateTime.UtcNow, DateTime.UtcNow.AddHours(1), "Sample Description");

        public async Task<IEnumerable<Appointment>> GetAsync()
        {
            return await Task.FromResult<IEnumerable<Appointment>>(new List<Appointment>
            {
                mockedAppointment
            });
        }

        public async Task<Appointment> GetAsync(int id)
        {
            //TODO: Get Rid of this hardcoded data
            return await Task.FromResult(mockedAppointment);
        }

        public async Task<Appointment> AddAsync(Appointment appointment)
        {
            return await Task.FromResult(mockedAppointment);
        }
    }
}