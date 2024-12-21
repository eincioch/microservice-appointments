using Microservice.Appointments.Api.Mappers;
using Microservice.Appointments.Api.Mappers.Abstractions;
using Microservice.Appointments.Application.UseCases.Mappers;
using Microservice.Appointments.Application.UseCases.Mappers.Abstractions;

namespace Microservice.Appointments.Api.DependencyInjection;

public static partial class DependencyInjection
{
    public static IServiceCollection AddMappers(this IServiceCollection services)
    {
        services.AddTransient<IExceptionToHttpMapper, ExceptionToHttpMapper>();
        services.AddSingleton<IAppointmentMapper, AppointmentMapper>();

        return services;
    }
}