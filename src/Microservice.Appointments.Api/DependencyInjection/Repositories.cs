using Microservice.Appointments.Application.Repositories;
using Microservice.Appointments.Infrastructure.Mappers;
using Microservice.Appointments.Infrastructure.Mappers.Abstractions;
using Microservice.Appointments.Infrastructure.Repositories;

namespace Microservice.Appointments.Api.DependencyInjection;

public static partial class DependencyInjection
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IAppointmentRepository, AppointmentRepository>();

        services.AddScoped<IAppointmentEntityMapper, AppointmentEntityMapper>();

        return services;
    }
}