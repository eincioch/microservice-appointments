using AutoFixture;
using Microservice.Appointments.Api.Helpers;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace Microservice.Appointments.UnitTests.Api.Helpers;

public class ActionResultHelperTests
{
    private readonly Fixture _fixture = new();

    private const string IdField = "id";

    [Fact]
    public void Given_Valid_Parameters_When_Created_Is_Called_Then_Returns_Correct_CreatedAtActionResult()
    {
        // Arrange
        var expectedActionName = _fixture.Create<string>();
        var expectedControllerName = _fixture.Create<string>();
        var expectedId = _fixture.Create<int>();
        var expectedResponse = _fixture.Create<object>();

        // Act
        var result = ActionResultHelper.Created(expectedActionName, expectedControllerName, expectedId, expectedResponse);

        // Assert
        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(expectedActionName, createdAtActionResult.ActionName);
        Assert.Equal(expectedControllerName, createdAtActionResult.ControllerName);
        Assert.Equal(expectedId, createdAtActionResult.RouteValues![IdField]);
        Assert.Equal(expectedResponse, createdAtActionResult.Value);
    }
}