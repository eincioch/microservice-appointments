using Microservice.Appointments.CrossCutting.Mappers.Abstractions;
using Microservice.Appointments.Domain.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;

namespace Microservice.Appointments.CrossCutting.Mappers;

public class ExceptionToHttpMapper : IExceptionToHttpMapper
{
    public (int StatusCode, string Title) Map(Exception exception) =>
        exception switch
        {
            BadRequestException => (StatusCodes.Status400BadRequest, ReasonPhrases.GetReasonPhrase(StatusCodes.Status400BadRequest)),
            NotFoundException => (StatusCodes.Status404NotFound, ReasonPhrases.GetReasonPhrase(StatusCodes.Status404NotFound)),
            _ => (StatusCodes.Status500InternalServerError, ReasonPhrases.GetReasonPhrase(StatusCodes.Status500InternalServerError))
        };
}