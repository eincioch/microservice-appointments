using System.Diagnostics;

namespace Microservice.Appointments.Api.Initializers.Base;

/// <summary>
/// Abstract base class that encapsulates the logic to start Docker services.
/// </summary>
/// <param name="environment">The environment name.</param>
public abstract class ServiceInitializerBase(string environment)
{
    private readonly string _environment = environment;

    protected virtual string DockerComposeCommand => "docker-compose";
    protected virtual string DockerComposeFile => $"docker-compose.{_environment}.yml";
    protected virtual string DockerComposeUpArguments => $"-f {DockerComposeFile} up -d";
    protected virtual string DockerComposeWorkingDirectory => Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", ".."));

    protected abstract string ContainerName { get; }
    protected abstract int StartupDelay { get; }

    public void InitializeContainer()
    {
        if (!IsContainerRunning())
        {
            StartDockerContainerProcess();
            Thread.Sleep(StartupDelay);
        }
    }

    private void StartDockerContainerProcess()
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = DockerComposeCommand,
            Arguments = DockerComposeUpArguments,
            WorkingDirectory = DockerComposeWorkingDirectory,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = Process.Start(startInfo);
        process?.WaitForExit();
    }

    /// <summary>
    /// Checks if the container is already running.
    /// </summary>
    /// <returns>True if the container is running.</returns>
    private bool IsContainerRunning()
    {
        var psi = new ProcessStartInfo
        {
            FileName = "docker",
            Arguments = $"ps --filter \"name={ContainerName}\" --format \"{{{{.Names}}}}\"",
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = Process.Start(psi);
        process?.WaitForExit();

        var output = process?.StandardOutput.ReadToEnd();
        return output?.Contains(ContainerName, StringComparison.OrdinalIgnoreCase) ?? false;
    }
}