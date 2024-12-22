using System.Text.Json;
using AutoFixture;
using Microservice.Appointments.Api.Mappers.Abstractions;
using Microservice.Appointments.Api.Middlewares;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Microservice.Appointments.UnitTests.Api.Middlewares;

public class ExceptionMiddlewareTests
{
    private readonly Fixture _fixture = new();

    private const long StreamStartPosition = 0;
    private const int InternalServerErrorStatusCode = StatusCodes.Status500InternalServerError;
    private const string InternalServerErrorMessage = "Internal Server Error";
    private const string UnhandledExceptionLogMessage = "An unhandled exception occurred.";

    [Fact]
    public async Task Given_UnhandledException_When_InvokeAsync_Then_Returns_InternalServerError()
    {
        // Arrange
        var mockLogger = new Mock<ILogger<ExceptionMiddleware>>();
        var mockMapper = new Mock<IExceptionToHttpMapper>();
        mockMapper.Setup(mapper => mapper.Map(It.IsAny<Exception>()))
            .Returns((InternalServerErrorStatusCode, InternalServerErrorMessage));

        var testExceptionMessage = _fixture.Create<string>();
        var middleware = new ExceptionMiddleware(_ => throw new Exception(testExceptionMessage), mockLogger.Object, mockMapper.Object);
        var context = new DefaultHttpContext
        {
            Response =
            {
                Body = new MemoryStream()
            }
        };

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        Assert.Equal(InternalServerErrorStatusCode, context.Response.StatusCode);

        context.Response.Body.Seek(StreamStartPosition, SeekOrigin.Begin);
        var responseBody = await new StreamReader(context.Response.Body).ReadToEndAsync();
        var problemDetails = JsonSerializer.Deserialize<ProblemDetails>(responseBody);
        Assert.Equal(InternalServerErrorMessage, problemDetails?.Title);
        Assert.Equal(testExceptionMessage, problemDetails?.Detail);

        mockLogger.Verify(
            logger => logger.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains(UnhandledExceptionLogMessage)),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
            Times.Once);
    }
}