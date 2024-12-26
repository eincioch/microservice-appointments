using AutoFixture;
using Microservice.Appointments.Api.Middlewares;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Microservice.Appointments.UnitTests.Api.Middlewares;

public class RequestLoggingMiddlewareTests
{
    private readonly Fixture _fixture = new();

    private const string IncomingRequestLogFormat = "Incoming request: {0} {1}";
    private const string OutgoingResponseLogFormat = "Outgoing response: {0}";
    private const string ValidRequestPath = "/test-path";

    [Fact]
    public async Task Given_Valid_Request_When_InvokeAsync_Called_Then_Logs_Incoming_Request_And_Outgoing_Response()
    {
        // Arrange
        var mockLogger = new Mock<ILogger<RequestLoggingMiddleware>>();
        var mockNext = new Mock<RequestDelegate>();

        mockNext.Setup(next => next(It.IsAny<HttpContext>()))
                .Returns(Task.CompletedTask);

        var middleware = new RequestLoggingMiddleware(mockNext.Object, mockLogger.Object);

        var httpMethod = _fixture.Create<string>();
        var responseStatusCode = StatusCodes.Status200OK;

        var context = new DefaultHttpContext
        {
            Request =
            {
                Method = httpMethod,
                Path = ValidRequestPath
            },
            Response =
            {
                StatusCode = responseStatusCode
            }
        };

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        mockLogger.Verify(
            logger => logger.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains(string.Format(IncomingRequestLogFormat, httpMethod, ValidRequestPath))),
                null,
                (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
            Times.Once);

        mockLogger.Verify(
            logger => logger.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains(string.Format(OutgoingResponseLogFormat, responseStatusCode))),
                null,
                (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
            Times.Once);

        mockNext.Verify(next => next(It.IsAny<HttpContext>()), Times.Once);
    }
}