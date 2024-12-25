using Microservice.Appointments.Api.HostedServices;
using Microservice.Appointments.Application.Configuration;
using Microservice.Appointments.Infrastructure.Configurations;

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