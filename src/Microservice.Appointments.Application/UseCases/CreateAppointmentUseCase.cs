using Microservice.Appointments.Application.Configuration;
using Microservice.Appointments.Application.Dtos.Appointments;
using Microservice.Appointments.Application.Helpers;
using Microservice.Appointments.Application.Repositories;
using Microservice.Appointments.Application.UseCases.Abstractions;
using Microservice.Appointments.Application.UseCases.Mappers.Abstractions;
using Microservice.Appointments.Domain.Events;
using Microservice.Appointments.Domain.Exceptions;
using Microservice.Appointments.Domain.Models;
using Microsoft.Extensions.Logging;

namespace Microservice.Appointments.Application.UseCases;

public class CreateAppointmentUseCase(
    IAppointmentRepository appointmentRepository,
    IAppointmentMapper appointmentMapper,
    IEventBus eventBus,
    ILogger<CreateAppointmentUseCase> logger) : ICreateAppointmentUseCase
{
    private readonly IAppointmentRepository _appointmentRepository = appointmentRepository ?? throw new ArgumentNullException(nameof(appointmentRepository));
    private readonly IAppointmentMapper _appointmentMapper = appointmentMapper ?? throw new ArgumentNullException(nameof(appointmentMapper));
    private readonly IEventBus _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
    private readonly ILogger<CreateAppointmentUseCase> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    private const string ValidationErrorMessage = "Validation error occurred while creating an appointment.";

    public async Task<AppointmentDto> ExecuteAsync(string title, DateTime startTime, DateTime endTime, string description)
    {
        try
        {
            var appointment = new AppointmentDomain(title, startTime, endTime, description);

            var appointmentSaved = await _appointmentRepository.AddAsync(appointment);
            appointment.AssignId(appointmentSaved.Id);

            var eventMessage = _appointmentMapper.ToAppointmentCreatedMessage(appointmentSaved);

            var eventName = EventHelper.GetEventName<AppointmentCreatedEvent>();
            await _eventBus.PublishAsync(eventMessage, eventName);

            return _appointmentMapper.ToDto(appointmentSaved);
        }
        catch (DomainValidationException exception)
        {
            _logger.LogWarning(exception, ValidationErrorMessage);
            throw new BadRequestException(ValidationErrorMessage, exception);
        }
    }
}