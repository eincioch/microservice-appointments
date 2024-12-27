using Microservice.Appointments.Api.Initializers;
using Microservice.Appointments.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Microservice.Appointments.Api.DependencyInjection;

public static class DbContext
{
    private const string DefaultConnectionStringName = "DefaultConnection";

    public static IServiceCollection AddDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString(DefaultConnectionStringName);
        services.AddDbContext<AppointmentsDbContext>(options =>
            options.UseSqlServer(connectionString));
        return services;
    }

    public static void AddMigrations(IServiceProvider serviceProvider)
    {
        var entityFrameworkInitializer = new EntityFrameworkInitializer(serviceProvider, serviceProvider.GetRequiredService<ILogger<EntityFrameworkInitializer>>());

        entityFrameworkInitializer.ApplyMigrations();
    }
}