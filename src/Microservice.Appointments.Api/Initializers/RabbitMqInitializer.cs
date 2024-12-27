using Microservice.Appointments.Api.Initializers.Base;

namespace Microservice.Appointments.Api.Initializers;

public class RabbitMqInitializer(string environment) : ServiceInitializerBase(environment)
{
    protected override string ContainerName => $"rabbitmq-{environment}";
    protected override int StartupDelay => 15000;
}