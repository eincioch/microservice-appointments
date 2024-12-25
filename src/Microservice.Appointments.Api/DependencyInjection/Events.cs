using Microservice.Appointments.Application.EventBus;
using Microservice.Appointments.Infrastructure.EventBus;
using Microservice.Appointments.Infrastructure.EventBus.HostedServices;

namespace Microservice.Appointments.Api.DependencyInjection;

public static partial class DependencyInjection
{
    public static IServiceCollection AddEvents(this IServiceCollection services, IConfiguration configuration)
    {
        //TODO: Use IConfiguration.
        var host = configuration["EventBus:Host"];
        var exchangeName = configuration["EventBus:ExchangeName"];
        var queueName = configuration["EventBus:QueueName"];

        var eventBus = RabbitMqEventBus.CreateAsync(host!, exchangeName!, queueName!)
            .ConfigureAwait(false)
            .GetAwaiter()
            .GetResult();

        services.AddSingleton<IEventBus>(eventBus);

        var routingMap = new Dictionary<Type, string>();

        services.AddSingleton(routingMap);
        services.AddHostedService<EventBusHostedService>();

        return services;
    }
}