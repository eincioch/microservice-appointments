using System.Threading.Channels;
using Microservice.Appointments.IntegrationTests.Configuration;

namespace Microservice.Appointments.IntegrationTests.Helpers;

public class EventBusHelper(IEventBus eventBus) : IAsyncDisposable
{
    private readonly IEventBus _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
    private readonly Channel<object> _channel = Channel.CreateUnbounded<object>();

    public async Task SubscribeToEventAsync<T>(string routingKey = null!) where T : class
    {
        await _eventBus.SubscribeAsync<T>(routingKey ?? string.Empty, @event =>
        {
            _channel.Writer.TryWrite(@event);
            return Task.CompletedTask;
        });
    }

    public async Task<T?> WaitForEventAsync<T>(Func<T, bool> matchCriteria, int timeoutSeconds = 5) where T : class
    {
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(timeoutSeconds));

        try
        {
            while (await _channel.Reader.WaitToReadAsync(cts.Token))
            {
                if (_channel.Reader.TryRead(out var @event) && @event is T typedEvent && matchCriteria(typedEvent))
                {
                    return typedEvent;
                }
            }
        }
        catch (OperationCanceledException)
        {
            return null;
        }

        return null;
    }

    public ValueTask DisposeAsync() => _eventBus.DisposeAsync();
}