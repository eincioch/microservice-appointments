using Microsoft.Extensions.Configuration;

namespace Microservice.Appointments.IntegrationTests.Core.Settings;

public class ConfigurationFile
{
    private const string FilePath = "appsettings.tests.json";
    private const string BaseUrlKey = "BaseUrl";
    private const string EventBusHostKey = "EventBus:Host";
    private const string EventBusPortKey = "EventBus:Port";
    private const string EventBusExchangeNameKey = "EventBus:ExchangeName";
    private const string EventBusQueueNameKey = "EventBus:QueueName";

    private static ConfigurationFile? _instance;

    private readonly IConfigurationRoot _configuration;

    private ConfigurationFile()
    {
        _configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile(FilePath)
            .Build();
    }

    public static ConfigurationFile Instance
    {
        get
        {
            if (_instance is null)
                _instance = new ConfigurationFile();

            return _instance;
        }
    }

    private T GetConfigValue<T>(string key) => _configuration.GetValue<T>(key)!;

    public string BaseUrl => GetConfigValue<string>(BaseUrlKey);
    public string EventBusHost => GetConfigValue<string>(EventBusHostKey);
    public int EventBusPort => GetConfigValue<int>(EventBusPortKey);
    public string EventBusExchangeName => GetConfigValue<string>(EventBusExchangeNameKey);
    public string EventBusQueueName => GetConfigValue<string>(EventBusQueueNameKey);
}