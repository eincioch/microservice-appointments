using Microservice.Appointments.Application.Repositories;
using Microservice.Appointments.Infrastructure.Repositories;

namespace Microservice.Appointments.Api.DependencyInjection;

public static partial class DependencyInjection
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IAppointmentRepository, AppointmentRepository>();

        return services;
    }
}
