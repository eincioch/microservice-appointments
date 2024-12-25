using Microservice.Appointments.Application.EventBus;
using Microservice.Appointments.Application.Helpers;
using Microservice.Appointments.Application.Repositories;
using Microservice.Appointments.Application.UseCases.Abstractions;
using Microservice.Appointments.Application.UseCases.Mappers.Abstractions;
using Microservice.Appointments.Domain.Events;
using Microservice.Appointments.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Microservice.Appointments.Application.UseCases;

public class DeleteAppointmentUseCase(
    IAppointmentRepository appointmentRepository,
    IAppointmentMapper appointmentMapper,
    IEventBus eventBus,
    ILogger<DeleteAppointmentUseCase> logger) : IDeleteAppointmentUseCase
{
    private readonly IAppointmentRepository _appointmentRepository = appointmentRepository ?? throw new ArgumentNullException(nameof(appointmentRepository));
    private readonly IAppointmentMapper _appointmentMapper = appointmentMapper ?? throw new ArgumentNullException(nameof(appointmentMapper));
    private readonly IEventBus _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
    private readonly ILogger<DeleteAppointmentUseCase> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    private const string ValidationErrorMessage = "Validation error occurred while deleting the appointment.";

    public async Task ExecuteAsync(int id)
    {
        try
        {
            var appointment = await _appointmentRepository.GetAsync(id);
            if (appointment is null)
                throw new NotFoundException($"Appointment with id '{id}' not found.");

            appointment.ValidateDeletable();

            await _appointmentRepository.RemoveAsync(appointment);

            var eventMessage = _appointmentMapper.ToDeletedMessage(appointment);

            var eventName = EventHelper.GetEventName<AppointmentDeletedEvent>();
            await _eventBus.PublishAsync(eventMessage, eventName);
        }
        catch (DomainValidationException exception)
        {
            _logger.LogWarning(exception, ValidationErrorMessage);
            throw new BadRequestException(ValidationErrorMessage, exception);
        }
    }
}