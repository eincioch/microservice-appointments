using Microsoft.Extensions.Configuration;

namespace Microservice.Appointments.IntegrationTests.Configuration;
public class ConfigurationFile
{
    private const string FilePath = "appsettings.tests.json";
    private const string BaseUrlKey = "BaseUrl";

    private static ConfigurationFile? _instance;

    private readonly IConfigurationRoot _configuration;
    private string? _baseUrl;

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

    public string BaseUrl
    {
        get
        {
            if (_baseUrl is null)
                _baseUrl = _configuration.GetValue<string>(BaseUrlKey)!;

            return _baseUrl;
        }
    }
}