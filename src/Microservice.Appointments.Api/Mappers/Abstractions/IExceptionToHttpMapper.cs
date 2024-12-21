namespace Microservice.Appointments.Api.Mappers.Abstractions;

public interface IExceptionToHttpMapper
{
    (int StatusCode, string Title) Map(Exception exception);
}