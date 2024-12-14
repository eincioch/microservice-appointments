using Microservice.Appointments.Application.UseCases;
using Microservice.Appointments.Application.UseCases.Abstractions;

namespace Microservice.Appointments.Api.DependencyInjection;

public static partial class DependencyInjection
{
    public static IServiceCollection AddUseCases(this IServiceCollection services)
    {
        services.AddScoped<IGetAppointmentsUseCase, GetAppointmentsUseCase>();

        return services;
    }
}
