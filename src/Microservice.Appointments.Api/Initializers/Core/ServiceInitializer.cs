using Microservice.Appointments.Api.Initializers.Base;

namespace Microservice.Appointments.Api.Initializers.Core;

public static class ServiceInitializer
{
    private static ILogger Logger => LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger(nameof(ServiceInitializer));
    private const string AppsettingsFileName = "appsettings";
    private const string AppsettingsExtension = "json";

    public static void UseConfiguration(IConfigurationBuilder configurationBuilder, string environment)
    {
        configurationBuilder
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile($".{AppsettingsExtension}", optional: true, reloadOnChange: true)
            .AddJsonFile($"{AppsettingsFileName}.{environment}.{AppsettingsExtension}", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables();
    }

    public static void InitializeContainers(string environment)
    {
        Logger.LogInformation($"Running in {environment} environment mode...");

        var environmentPattern = environment.ToLower();
        var initializers = new ServiceInitializerBase[]
        {
            new RabbitMqInitializer(environmentPattern),
            new SqlServerInitializer(environmentPattern)
        };

        Array.ForEach(initializers, service => service.InitializeContainer());
    }

}