namespace Microservice.Appointments.Domain.Exceptions;

public class DomainValidationException(string message) : Exception(message);