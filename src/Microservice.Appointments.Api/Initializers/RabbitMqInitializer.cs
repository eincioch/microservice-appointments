using System.Diagnostics;

namespace Microservice.Appointments.Api.Initializers;

public static class RabbitMqInitializer
{
    private const string RabbitMqContainerName = "rabbitmq";
    private const string DockerComposeCommand = "docker-compose";
    private const string DockerComposeUpArguments = "up -d";
    private const int RabbitMqStartupDelay = 10000;

    private const string RabbitMqNotRunningMessage = "RabbitMQ is not running. Starting RabbitMQ container...";
    private const string RabbitMqStartedMessage = "RabbitMQ container started. Waiting for it to be ready...";
    private const string RabbitMqReadyMessage = "RabbitMQ is ready to start.";
    private const string RabbitMqAlreadyRunningMessage = "RabbitMQ is already running.";

    private static readonly ILogger _logger;

    static RabbitMqInitializer()
    {
        using var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        _logger = loggerFactory.CreateLogger(nameof(RabbitMqInitializer));
    }

    /// <summary>
    /// Sets up RabbitMQ by ensuring the container is running and ready.
    /// </summary>
    public static void SetupRabbitMq()
    {
        if (!IsRabbitMqRunning())
        {
            _logger.LogInformation(RabbitMqNotRunningMessage);

            var dockerUp = Process.Start(DockerComposeCommand, DockerComposeUpArguments);
            dockerUp.WaitForExit();

            _logger.LogInformation(RabbitMqStartedMessage);

            Thread.Sleep(RabbitMqStartupDelay);

            _logger.LogInformation(RabbitMqReadyMessage);
            return;
        }

        _logger.LogInformation(RabbitMqAlreadyRunningMessage);
    }

    /// <summary>
    /// Checks if the RabbitMQ container is running.
    /// </summary>
    /// <returns>True if the container is running; otherwise, false.</returns>
    private static bool IsRabbitMqRunning()
    {
        var dockerPs = Process.Start(new ProcessStartInfo
        {
            FileName = "docker",
            Arguments = $"ps --filter \"name={RabbitMqContainerName}\" --filter \"status=running\" --format \"{{{{.Names}}}}\"",
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        });

        if (dockerPs == null) return false;

        var output = dockerPs.StandardOutput.ReadToEnd();
        dockerPs.WaitForExit();

        return output.Contains(RabbitMqContainerName, StringComparison.OrdinalIgnoreCase);
    }
}