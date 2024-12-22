using Microservice.Appointments.Api.Initializers.Base;

namespace Microservice.Appointments.Api.Initializers;

public class SqlServerInitializer : ServiceInitializerBase
{
    protected override string ContainerName => "sqlserver";
    protected override int StartupDelay => 30000;
}