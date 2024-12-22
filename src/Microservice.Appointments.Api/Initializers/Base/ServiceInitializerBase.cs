using System.Diagnostics;

namespace Microservice.Appointments.Api.Initializers.Base;

/// <summary>
/// Abstract base class that encapsulates the logic to start Docker services.
/// </summary>
public abstract class ServiceInitializerBase
{
    protected virtual string DockerComposeCommand => "docker-compose";
    protected virtual string DockerComposeUpArguments => "up -d";
    protected abstract string ContainerName { get; }
    protected abstract int StartupDelay { get; }

    protected ILogger Logger { get; }

    protected ServiceInitializerBase()
    {
        using var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        Logger = loggerFactory.CreateLogger(GetType().Name);
    }

    /// <summary>
    /// Main method to start the service if it's not running.
    /// </summary>
    public virtual void SetupService()
    {
        if (!IsServiceRunning())
        {
            Logger.LogInformation($"{ContainerName} container is not running. Starting via docker-compose...");
            using var dockerUp = Process.Start(DockerComposeCommand, DockerComposeUpArguments);
            dockerUp?.WaitForExit();

            Logger.LogInformation($"{ContainerName} container started. Waiting {StartupDelay}ms for it to be ready...");
            Thread.Sleep(StartupDelay);

            Logger.LogInformation($"{ContainerName} should be ready now.");
        }
        else
        {
            Logger.LogInformation($"{ContainerName} is already running.");
        }
    }

    /// <summary>
    /// Checks if the container is running (docker ps).
    /// </summary>
    /// <returns>true if it's running; false otherwise.</returns>
    protected virtual bool IsServiceRunning()
    {
        var dockerPs = Process.Start(new ProcessStartInfo
        {
            FileName = "docker",
            Arguments = $"ps --filter \"name={ContainerName}\" --filter \"status=running\" --format \"{{{{.Names}}}}\"",
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        });

        if (dockerPs == null) return false;

        var output = dockerPs.StandardOutput.ReadToEnd();
        dockerPs.WaitForExit();

        return output.Contains(ContainerName, StringComparison.OrdinalIgnoreCase);
    }
}