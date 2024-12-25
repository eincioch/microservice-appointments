using Microservice.Appointments.Application.EventBus.Handlers;
using Microservice.Appointments.Application.Repositories;
using Microservice.Appointments.Domain.Events;
using Microsoft.Extensions.Logging;

namespace Microservice.Appointments.Application.EventHandlers;

public class AppointmentNotificationHandler(IAppointmentRepository appointmentRepository, ILogger<AppointmentNotificationHandler> logger) : IEventHandler<AppointmentNotificationEvent>
{
    private readonly IAppointmentRepository _appointmentRepository = appointmentRepository ?? throw new ArgumentNullException(nameof(appointmentRepository));
    private readonly ILogger<AppointmentNotificationHandler> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    private const string NotificationEventType = "AppointmentNotificationEvent";

    public async Task HandleAsync(AppointmentNotificationEvent @event, CancellationToken cancellationToken = default)
    {
        if (@event.Type != NotificationEventType)
            return;

        var appointment = await _appointmentRepository.GetAsync(@event.AppointmentId);
        if (appointment is null)
        {
            _logger.LogWarning($"Appointment with ID {@event.AppointmentId} not found.");
            return;
        }

        appointment.MarkNotificationSent();
        await _appointmentRepository.UpdateAsync(appointment);

        _logger.LogInformation($"Notification processed for appointment: {@event.AppointmentId}");
    }
}