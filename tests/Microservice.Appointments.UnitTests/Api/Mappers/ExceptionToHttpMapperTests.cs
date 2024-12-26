using AutoFixture;
using Microservice.Appointments.Api.Mappers;
using Microservice.Appointments.Api.Mappers.Abstractions;
using Microservice.Appointments.Domain.Exceptions;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace Microservice.Appointments.UnitTests.Api.Mappers;

public class ExceptionToHttpMapperTests
{
    private readonly Fixture _fixture = new();

    private const string BadRequestTitle = "Bad Request";
    private const string NotFoundTitle = "Not Found";
    private const string InternalServerErrorTitle = "Internal Server Error";

    [Fact]
    public void Given_BadRequestException_When_Map_Called_Then_Returns_Bad_Request()
    {
        // Arrange
        var mapper = new ExceptionToHttpMapper();
        var exception = new BadRequestException(_fixture.Create<string>());

        // Act
        var (statusCode, title) = mapper.Map(exception);

        // Assert
        Assert.Equal(StatusCodes.Status400BadRequest, statusCode);
        Assert.Equal(BadRequestTitle, title);
    }

    [Fact]
    public void Given_NotFoundException_When_Map_Called_Then_Returns_Not_Found()
    {
        // Arrange
        var mapper = new ExceptionToHttpMapper();
        var exception = new NotFoundException(_fixture.Create<string>());

        // Act
        var (statusCode, title) = mapper.Map(exception);

        // Assert
        Assert.Equal(StatusCodes.Status404NotFound, statusCode);
        Assert.Equal(NotFoundTitle, title);
    }

    [Fact]
    public void Given_Generic_Exception_When_Map_Called_Then_Returns_Internal_Server_Error()
    {
        // Arrange
        var mapper = new ExceptionToHttpMapper();
        var exception = new Exception(_fixture.Create<string>());

        // Act
        var (statusCode, title) = mapper.Map(exception);

        // Assert
        Assert.Equal(StatusCodes.Status500InternalServerError, statusCode);
        Assert.Equal(InternalServerErrorTitle, title);
    }

    [Fact]
    public void Given_ExceptionToHttpMapper_When_Checked_Then_Implements_IExceptionToHttpMapper()
    {
        // Arrange & Act
        var mapper = new ExceptionToHttpMapper();

        // Assert
        Assert.IsAssignableFrom<IExceptionToHttpMapper>(mapper);
    }
}