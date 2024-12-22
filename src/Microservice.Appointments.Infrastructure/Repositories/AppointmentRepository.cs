using Microservice.Appointments.Application.Repositories;
using Microservice.Appointments.Domain.Models;
using Microservice.Appointments.Infrastructure.Mappers.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Microservice.Appointments.Infrastructure.Repositories
{
    public class AppointmentRepository(AppointmentsDbContext context, IAppointmentEntityMapper appointmentEntityMapper) : IAppointmentRepository
    {
        private readonly AppointmentsDbContext _context = context ?? throw new ArgumentNullException(nameof(context));
        private readonly IAppointmentEntityMapper _appointmentEntityMapper = appointmentEntityMapper ?? throw new ArgumentNullException(nameof(appointmentEntityMapper));

        public async Task<IEnumerable<AppointmentDomain>> GetAsync()
        {
            var appointments = await _context.Appointments.ToListAsync();
            return appointmentEntityMapper.ToDomainCollection(appointments);
        }

        public async Task<AppointmentDomain> GetAsync(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            return appointmentEntityMapper.ToDomain(appointment!);
        }

        public async Task<AppointmentDomain> AddAsync(AppointmentDomain appointmentDomain)
        {
            var entity = appointmentEntityMapper.ToEntity(appointmentDomain);
            _context.Appointments.Add(entity);
            await _context.SaveChangesAsync();
            return appointmentEntityMapper.ToDomain(entity);
        }
    }
}