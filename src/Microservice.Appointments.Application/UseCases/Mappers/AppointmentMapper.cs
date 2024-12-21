using Microservice.Appointments.Application.Dtos.Appointments;
using Microservice.Appointments.Application.UseCases.Mappers.Abstractions;
using Microservice.Appointments.Domain.Events;
using Microservice.Appointments.Domain.Models;

namespace Microservice.Appointments.Application.UseCases.Mappers;

public class AppointmentMapper : IAppointmentMapper
{
    public AppointmentDto ToDto(Appointment appointment)
        => new(
            appointment.Id,
            appointment.Title,
            appointment.StartTime,
            appointment.EndTime,
            appointment.Description,
            appointment.Status
        );

    public AppointmentCreatedEvent ToAppointmentCreatedMessage(Appointment appointment)
        => new(
            appointment.Id,
            appointment.Title,
            appointment.StartTime,
            appointment.EndTime,
            appointment.Description,
            appointment.Status
        );
}