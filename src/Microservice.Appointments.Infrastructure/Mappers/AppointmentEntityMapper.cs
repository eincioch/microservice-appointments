using Microservice.Appointments.Domain.Models;
using Microservice.Appointments.Infrastructure.Entities;
using Microservice.Appointments.Infrastructure.Mappers.Abstractions;

namespace Microservice.Appointments.Infrastructure.Mappers;

public class AppointmentEntityMapper : IAppointmentEntityMapper
{
    public AppointmentDomain ToDomain(Appointment entity)
        => entity is null ? null : AppointmentDomain.Hydrate(entity.Id, entity.Title, entity.StartTime, entity.EndTime, entity.Description, entity.Status);

    public Appointment ToEntity(AppointmentDomain domain)
        => domain is null
            ? null : new()
            {
                Id = domain.Id,
                Title = domain.Title,
                StartTime = domain.StartTime,
                EndTime = domain.EndTime,
                Description = domain.Description,
                Status = domain.Status
            };
}