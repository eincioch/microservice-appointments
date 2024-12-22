using Microservice.Appointments.Application.Repositories;
using Microservice.Appointments.Domain.Models;
using Microservice.Appointments.Infrastructure.Entities;
using Microservice.Appointments.Infrastructure.Mappers.Abstractions;
using Microservice.Appointments.Infrastructure.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace Microservice.Appointments.Infrastructure.Repositories
{
    public class AppointmentRepository(AppointmentsDbContext context, IAppointmentEntityMapper appointmentEntityMapper) : RepositoryBase<AppointmentsDbContext, Appointment>(context), IAppointmentRepository
    {
        private readonly IAppointmentEntityMapper _appointmentEntityMapper = appointmentEntityMapper ?? throw new ArgumentNullException(nameof(appointmentEntityMapper));

        public async Task<IEnumerable<AppointmentDomain>> GetAsync()
        {
            var appointments = await _context.Appointments.ToListAsync();
            return _appointmentEntityMapper.ToDomainCollection(appointments);
        }

        public async Task<AppointmentDomain> GetAsync(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            return _appointmentEntityMapper.ToDomain(appointment!);
        }

        public async Task<AppointmentDomain> AddAsync(AppointmentDomain appointmentDomain)
        {
            var entity = _appointmentEntityMapper.ToEntity(appointmentDomain);
            _context.Appointments.Add(entity);
            await _context.SaveChangesAsync();
            return _appointmentEntityMapper.ToDomain(entity);
        }

        public async Task<AppointmentDomain> UpdateAsync(AppointmentDomain appointmentDomain)
        {
            var entity = _appointmentEntityMapper.ToEntity(appointmentDomain);
            DetachEntity(entity.Id);
            _context.Appointments.Update(entity);
            await _context.SaveChangesAsync();
            return _appointmentEntityMapper.ToDomain(entity);
        }
    }
}