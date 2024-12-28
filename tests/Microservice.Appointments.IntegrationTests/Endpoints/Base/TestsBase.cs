using Microservice.Appointments.IntegrationTests.Configuration;
using Microservice.Appointments.IntegrationTests.Core.Infrastructure;
using Microservice.Appointments.IntegrationTests.Core.Settings;
using RestSharp;

namespace Microservice.Appointments.IntegrationTests.Endpoints.Base;

public class TestsBase
{
    protected readonly RestClient Client = new(ConfigurationFile.Instance.BaseUrl);
    protected readonly string EventBusQueueName = ConfigurationFile.Instance.EventBusQueueName;
    private IEventBus? _eventBus;

    protected IEventBus EventBus => _eventBus ??= RabbitMqEventBus
        .CreateAsync(
            ConfigurationFile.Instance.EventBusHost,
            ConfigurationFile.Instance.EventBusPort,
            ConfigurationFile.Instance.EventBusExchangeName,
            EventBusQueueName
        )
        .ConfigureAwait(false)
        .GetAwaiter()
        .GetResult();
}