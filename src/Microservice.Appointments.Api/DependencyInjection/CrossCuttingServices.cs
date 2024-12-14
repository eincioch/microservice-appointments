using Microservice.Appointments.CrossCutting.Mappers;
using Microservice.Appointments.CrossCutting.Mappers.Abstractions;

namespace Microservice.Appointments.Api.DependencyInjection;

public static partial class DependencyInjection
{
    public static IServiceCollection AddCrossCuttingServices(this IServiceCollection services)
    {
        services.AddTransient<IExceptionToHttpMapper, ExceptionToHttpMapper>();

        return services;
    }
}