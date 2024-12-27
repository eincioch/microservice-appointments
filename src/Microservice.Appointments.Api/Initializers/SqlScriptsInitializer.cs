using System.Diagnostics;

namespace Microservice.Appointments.Api.Initializers;

public class SqlScriptsInitializer(ILogger logger)
{
    private readonly ILogger _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    //TODO: Read This From System Scripts
    private const string DropDatabaseSql = @"
        USE master;
        IF EXISTS(SELECT * FROM sys.databases WHERE name = 'AppointmentsTest')
        BEGIN
            ALTER DATABASE [AppointmentsTest] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
            DROP DATABASE [AppointmentsTest];
        END;";


    public void ApplyScripts()
    {
        _logger.LogInformation("Applying SQL scripts...");
        ExecuteSqlScripts("IntegrityAssurance/Scripts/System");
        ExecuteSqlScripts("IntegrityAssurance/Scripts/Tests");
    }

    //TODO: Change this method to read system scripts
    public void DropAndCreateDatabase()
    {
        ExecuteSqlCommand(DropDatabaseSql);
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

        _logger.LogInformation($"SQL Command ExitCode: {process?.ExitCode}");
    }
}