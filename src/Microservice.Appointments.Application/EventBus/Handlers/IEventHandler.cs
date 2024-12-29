namespace Microservice.Appointments.Application.EventBus.Handlers;

public interface IEventHandler<in TEvent> where TEvent : class
{
    Task HandleAsync(TEvent apointmentNotificationEvent, CancellationToken cancellationToken = default);
}