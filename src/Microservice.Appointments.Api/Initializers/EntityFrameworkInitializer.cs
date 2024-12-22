using Microservice.Appointments.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Microservice.Appointments.Api.Initializers
{
    public class EntityFrameworkInitializer(IServiceProvider serviceProvider, ILogger<EntityFrameworkInitializer> logger)
    {
        private readonly IServiceProvider _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        private readonly ILogger<EntityFrameworkInitializer> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        private const string CheckingMigrationsMessage = "Checking and applying database migrations...";
        private const string MigrationsSuccessMessage = "Database migrations applied successfully.";
        private const string MigrationsErrorMessage = "An error occurred while applying database migrations.";

        public void ApplyMigrations()
        {
            try
            {
                _logger.LogInformation(CheckingMigrationsMessage);

                using var scope = _serviceProvider.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<AppointmentsDbContext>();

                dbContext.Database.Migrate();

                _logger.LogInformation(MigrationsSuccessMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, MigrationsErrorMessage);
                throw;
            }
        }
    }
}