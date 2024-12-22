using Microservice.Appointments.Application.UseCases;
using Microservice.Appointments.Application.UseCases.Abstractions;

namespace Microservice.Appointments.Api.DependencyInjection;

public static partial class DependencyInjection
{
    public static IServiceCollection AddUseCases(this IServiceCollection services)
    {
        services.AddScoped<IGetAppointmentsUseCase, GetAppointmentsUseCase>();
        services.AddScoped<IGetAppointmentByIdUseCase, GetAppointmentByIdUseCase>();
        services.AddScoped<ICreateAppointmentUseCase, CreateAppointmentUseCase>();
        services.AddScoped<IUpdateAppointmentUseCase, UpdateAppointmentUseCase>();

        return services;
    }
}