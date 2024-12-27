using Microservice.Appointments.Api.Initializers.Base;

namespace Microservice.Appointments.Api.Initializers;

public class SqlServerInitializer(string environment) : ServiceInitializerBase(environment)
{
    protected override string ContainerName => $"rabbitmq-{environment}";
    protected override int StartupDelay => 30000;
}