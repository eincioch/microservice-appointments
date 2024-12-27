using System.Diagnostics;

namespace Microservice.Appointments.Api.Initializers;

public class SqlScriptsRunnerInitializer(ILogger logger)
{
    private readonly ILogger _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    private const string ResetScriptsPath = "Scripts/Reset";
    private const string TestDataScriptsPath = "Scripts";
    private const string PreExecutionScriptsPath = "Scripts/PreExecution";

    public void ApplyTestScripts()
    {
        _logger.LogInformation("Applying SQL Test Scripts...");
        ExecuteSqlScripts(ResetScriptsPath);
        ExecuteSqlScripts(TestDataScriptsPath);
    }

    public void ApplySetupScripts()
    {
        _logger.LogInformation("Applying SQL Pre-Execution Tests Scripts...");
        ExecuteSqlScripts(PreExecutionScriptsPath);
    }

    private void ExecuteSqlScripts(string directory)
    {
        if (!Directory.Exists(directory)) return;

        var scripts = Directory.GetFiles(directory, "*.sql");
        foreach (var script in scripts)
        {
            ExecuteSqlScript(script);
        }
    }

    private void ExecuteSqlScript(string scriptPath)
    {
        ExecuteSqlCommand($"-i \"{scriptPath}\"");
    }

    private void ExecuteSqlCommand(string arguments)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = "sqlcmd",
            Arguments = $"-S localhost,1434 -U sa -P YourStrong!Passw0rd {arguments}",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = Process.Start(startInfo);
        process?.WaitForExit();

        var stdout = process?.StandardOutput.ReadToEnd();
        var stderr = process?.StandardError.ReadToEnd();

        _logger.LogInformation($"[SqlScriptsRunnerInitializer] SQL Command ExitCode: {process?.ExitCode}");

        if (!string.IsNullOrWhiteSpace(stdout))
            _logger.LogInformation($"[SqlScriptsRunnerInitializer] STDOUT:\n{stdout}");
        if (!string.IsNullOrWhiteSpace(stderr))
            _logger.LogError($"[SqlScriptsRunnerInitializer] STDERR:\n{stderr}");
    }
}