namespace Microservice.Appointments.IntegrationTests.Configuration;

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

    /// <summary>
    /// Subscribes to an event message asynchronously from the underlying message broker.
    /// </summary>
    /// <typeparam name="T">The type of the event payload.</typeparam>
    /// <param name="routingKey">The routing key to use for subscribing to the event.</param>
    /// <param name="handler">The handler function to process the event message.</param>
    Task SubscribeAsync<T>(string routingKey, Func<T, Task> handler);
}