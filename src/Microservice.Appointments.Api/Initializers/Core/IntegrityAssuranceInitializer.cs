namespace Microservice.Appointments.Api.Initializers.Core;

public static class IntegrityAssuranceInitializer
{
    private const string IntegrityAssuranceMode = "integrityassurance";
    private static ILogger Logger => LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger(nameof(ServiceInitializer));

    public static void ExecuteScripts(string environment)
    {
        if (string.Equals(environment, IntegrityAssuranceMode, StringComparison.OrdinalIgnoreCase))
        {
            var sqlScriptsInitializer = new SqlScriptsInitializer(Logger);
            sqlScriptsInitializer.ApplyScripts();
        }
    }

    public static void RecreateDatabase(string environment)
    {
        if (string.Equals(environment, IntegrityAssuranceMode, StringComparison.OrdinalIgnoreCase))
        {
            var sqlScriptsInitializer = new SqlScriptsInitializer(Logger);
            sqlScriptsInitializer.DropAndCreateDatabase();
        }
    }
}