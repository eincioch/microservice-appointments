using Microservice.Appointments.Domain.Models;
using Microservice.Appointments.Infrastructure.Entities;

namespace Microservice.Appointments.Infrastructure.Mappers.Abstractions;

public interface IAppointmentEntityMapper
{
    IEnumerable<AppointmentDomain> ToDomainCollection(IEnumerable<Appointment> entity);
    AppointmentDomain ToDomain(Appointment entity);
    Appointment ToEntity(AppointmentDomain domain);
}