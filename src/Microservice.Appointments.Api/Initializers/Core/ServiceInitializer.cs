using Microservice.Appointments.Api.Initializers.Base;

namespace Microservice.Appointments.Api.Initializers.Core;

public static class ServiceInitializer
{
    public static void InitializeInfrastructure(IServiceProvider serviceProvider)
    {
        var initializers = new ServiceInitializerBase[]
        {
            new RabbitMqInitializer(),
            new SqlServerInitializer()
        };

        Array.ForEach(initializers, service => service.SetupService());

        var entityFrameworkInitializer = new EntityFrameworkInitializer(
            serviceProvider,
            serviceProvider.GetRequiredService<ILogger<EntityFrameworkInitializer>>());

        entityFrameworkInitializer.ApplyMigrations();
    }
}