namespace Microservice.Appointments.Application.Configuration;

/// <summary>
/// Represents an asynchronous event bus that can publish messages to a message broker.
/// </summary>
public interface IEventBus : IAsyncDisposable
{
    /// <summary>
    /// Publishes an event message asynchronously to the underlying message broker.
    /// </summary>
    /// <typeparam name="T">The type of the event payload.</typeparam>
    /// <param name="event">The event payload to publish.</param>
    /// <param name="routingKey">The routing key to use when publishing the event.</param>
    Task PublishAsync<T>(T @event, string routingKey);
}