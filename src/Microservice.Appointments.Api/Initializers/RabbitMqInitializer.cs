using Microservice.Appointments.Api.Initializers.Base;

namespace Microservice.Appointments.Api.Initializers;

public class RabbitMqInitializer : ServiceInitializerBase
{
    protected override string ContainerName => "rabbitmq";
    protected override int StartupDelay => 10000;
}