namespace Microservice.Appointments.Api.Initializers.Core;

public static class IntegrityAssuranceInitializer
{
    private const string IntegrityAssuranceMode = "integrityassurance";
    private static ILogger Logger => LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger(nameof(ServiceInitializer));

    public static void RunTestScripts(string environment)
    {
        if (string.Equals(environment, IntegrityAssuranceMode, StringComparison.OrdinalIgnoreCase))
        {
            var sqlScriptsInitializer = new SqlScriptsRunnerInitializer(Logger);
            sqlScriptsInitializer.ApplyTestScripts();
        }
    }

    public static void RunSetupScripts(string environment)
    {
        if (string.Equals(environment, IntegrityAssuranceMode, StringComparison.OrdinalIgnoreCase))
        {
            var sqlScriptsInitializer = new SqlScriptsRunnerInitializer(Logger);
            sqlScriptsInitializer.ApplySetupScripts();
        }
    }
}