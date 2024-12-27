using Microservice.Appointments.Application.EventBus;
using Microservice.Appointments.Application.EventBus.Handlers;
using Microservice.Appointments.Application.EventHandlers;
using Microservice.Appointments.Application.Helpers;
using Microservice.Appointments.Domain.Events;
using Microservice.Appointments.Infrastructure.EventBus;
using Microservice.Appointments.Infrastructure.EventBus.HostedServices;

namespace Microservice.Appointments.Api.DependencyInjection;

public static partial class DependencyInjection
{
    public static IServiceCollection AddEvents(this IServiceCollection services, IConfiguration configuration)
    {
        //TODO: Use IConfiguration.
        var host = configuration["EventBus:Host"];
        var port = Convert.ToInt32(configuration["EventBus:Port"]);
        var exchangeName = configuration["EventBus:ExchangeName"];
        var queueName = configuration["EventBus:QueueName"];

        var eventBus = RabbitMqEventBus.CreateAsync(host!, port, exchangeName!, queueName!)
            .ConfigureAwait(false)
            .GetAwaiter()
            .GetResult();

        services.AddSingleton<IEventBus>(eventBus);

        services.AddScoped<IEventHandler<AppointmentNotificationEvent>, AppointmentNotificationHandler>();

        var routingMap = new Dictionary<Type, string>
        {
            { typeof(AppointmentNotificationEvent), EventHelper.GetEventName<AppointmentNotificationEvent>() }
        };

        services.AddSingleton(routingMap);
        services.AddHostedService<EventBusHostedService>();

        return services;
    }
}