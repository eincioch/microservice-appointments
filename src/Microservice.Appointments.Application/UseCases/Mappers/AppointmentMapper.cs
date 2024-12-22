using Microservice.Appointments.Application.Dtos.Appointments;
using Microservice.Appointments.Application.UseCases.Mappers.Abstractions;
using Microservice.Appointments.Domain.Events;
using Microservice.Appointments.Domain.Models;

namespace Microservice.Appointments.Application.UseCases.Mappers;

public class AppointmentMapper : IAppointmentMapper
{
    public AppointmentDto ToDto(AppointmentDomain appointmentDomain)
        => new(
            appointmentDomain.Id,
            appointmentDomain.Title,
            appointmentDomain.StartTime,
            appointmentDomain.EndTime,
            appointmentDomain.Description,
            appointmentDomain.Status
        );

    public AppointmentCreatedEvent ToCreatedMessage(AppointmentDomain appointmentDomain)
        => new(
            appointmentDomain.Id,
            appointmentDomain.Title,
            appointmentDomain.StartTime,
            appointmentDomain.EndTime,
            appointmentDomain.Description,
            appointmentDomain.Status
        );

    public AppointmentChangedEvent ToChangedMessage(AppointmentDomain appointmentDomain)
        => new(
            appointmentDomain.Id,
            appointmentDomain.Title,
            appointmentDomain.StartTime,
            appointmentDomain.EndTime,
            appointmentDomain.Description,
            appointmentDomain.Status
        );
}