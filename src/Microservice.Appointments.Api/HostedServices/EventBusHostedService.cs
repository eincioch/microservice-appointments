using System.Reflection;
using Microservice.Appointments.Application.Configuration;
using Microservice.Appointments.Application.EventBus.Handlers;

namespace Microservice.Appointments.Api.HostedServices;

public class EventBusHostedService(IEventBus eventBus, IServiceScopeFactory scopeFactory, Dictionary<Type, string> routingMap) : IHostedService
{
    private readonly IEventBus _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
    private readonly IServiceScopeFactory _scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
    private readonly Dictionary<Type, string> _routingMap = routingMap ?? throw new ArgumentNullException(nameof(routingMap));

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        foreach (var (eventType, routingKey) in _routingMap)
        {
            var subscribeMethod = typeof(EventBusHostedService)
                .GetMethod(nameof(SubscribeForEvent), BindingFlags.Instance | BindingFlags.NonPublic);

            var genericMethod = subscribeMethod!.MakeGenericMethod(eventType);
            genericMethod.Invoke(this, [routingKey]);
        }

        await Task.CompletedTask;
    }

    private void SubscribeForEvent<TEvent>(string routingKey) where TEvent : class
    {
        _ = _eventBus.SubscribeAsync<TEvent>(routingKey, async (@event) =>
        {
            using var scope = _scopeFactory.CreateScope();
            var handlers = scope.ServiceProvider.GetServices<IEventHandler<TEvent>>();
            foreach (var handler in handlers)
                await handler.HandleAsync(@event);
        });
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}