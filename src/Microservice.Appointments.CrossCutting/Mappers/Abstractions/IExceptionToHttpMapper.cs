namespace Microservice.Appointments.CrossCutting.Mappers.Abstractions;

public interface IExceptionToHttpMapper
{
    (int StatusCode, string Title) Map(Exception exception);
}