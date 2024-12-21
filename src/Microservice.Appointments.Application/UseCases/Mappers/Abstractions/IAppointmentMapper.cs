using Microservice.Appointments.Application.Dtos.Appointments;
using Microservice.Appointments.Domain.Events;
using Microservice.Appointments.Domain.Models;

namespace Microservice.Appointments.Application.UseCases.Mappers.Abstractions;

public interface IAppointmentMapper
{
    AppointmentDto ToDto(AppointmentDomain appointmentDomain);
    AppointmentCreatedEvent ToAppointmentCreatedMessage(AppointmentDomain appointmentDomain);
}