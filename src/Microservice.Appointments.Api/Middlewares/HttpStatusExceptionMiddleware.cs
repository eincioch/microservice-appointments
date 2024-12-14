using System.Net.Mime;
using Microservice.Appointments.CrossCutting.Mappers.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace Microservice.Appointments.Api.Middlewares;

public class ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IExceptionToHttpMapper exceptionMapper)
{
    private readonly RequestDelegate _next = next;
    private readonly ILogger<ExceptionMiddleware> _logger = logger;
    private readonly IExceptionToHttpMapper _exceptionMapper = exceptionMapper;

    private const string UnhandledExceptionMessage = "An unhandled exception occurred.";

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, UnhandledExceptionMessage);

            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, title) = _exceptionMapper.Map(exception);

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = exception.Message,
            Instance = context.Request.Path
        };

        context.Response.ContentType = MediaTypeNames.Application.Json;
        context.Response.StatusCode = statusCode;

        await context.Response.WriteAsJsonAsync(problemDetails);
    }
}