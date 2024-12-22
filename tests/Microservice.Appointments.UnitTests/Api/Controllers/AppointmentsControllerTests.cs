using AutoFixture;
using AutoFixture.AutoMoq;
using Microservice.Appointments.Api.Controllers;
using Microservice.Appointments.Application.Dtos.Appointments;
using Microservice.Appointments.Application.UseCases.Abstractions;
using Microservice.Appointments.Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Microservice.Appointments.UnitTests.Api.Controllers;

public class AppointmentsControllerTests
{
    #region Builder

    private class Builder
    {
        public Fixture Fixture { get; }
        public const string ControllerName = "Appointments";

        public Builder()
        {
            Fixture = new Fixture();
            Fixture.Customize(new AutoMoqCustomization { ConfigureMembers = true });
        }

        public AppointmentsController Build(
            Mock<IGetAppointmentsUseCase> getAppointmentsUseCase,
            Mock<IGetAppointmentByIdUseCase> getAppointmentByIdUseCase,
            Mock<ICreateAppointmentUseCase> createAppointmentUseCase,
            Mock<IUpdateAppointmentUseCase> updateAppointmentUseCase,
            Mock<IUpdateAppointmentStatusUseCase> updateAppointmentStatusUseCase,
            Mock<IDeleteAppointmentUseCase> deleteAppointmentUseCase)
        {
            return new AppointmentsController(
                getAppointmentsUseCase.Object,
                getAppointmentByIdUseCase.Object,
                createAppointmentUseCase.Object,
                updateAppointmentUseCase.Object,
                updateAppointmentStatusUseCase.Object,
                deleteAppointmentUseCase.Object
            )
            {
                ControllerContext = new ControllerContext
                {
                    ActionDescriptor = new Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor
                    {
                        ControllerName = ControllerName
                    }
                }
            };
        }
    }

    #endregion Builder

    [Fact]
    public async Task Given_Valid_Request_When_GetAll_Then_Returns_Ok()
    {
        // Arrange
        var builder = new Builder();
        var mockGetAppointmentsUseCase = new Mock<IGetAppointmentsUseCase>();
        var appointments = builder.Fixture.CreateMany<AppointmentDto>();
        mockGetAppointmentsUseCase
            .Setup(useCase => useCase.ExecuteAsync())
            .ReturnsAsync(appointments);

        var controller = builder.Build(
            mockGetAppointmentsUseCase,
            new Mock<IGetAppointmentByIdUseCase>(),
            new Mock<ICreateAppointmentUseCase>(),
            new Mock<IUpdateAppointmentUseCase>(),
            new Mock<IUpdateAppointmentStatusUseCase>(),
            new Mock<IDeleteAppointmentUseCase>()
        );

        // Act
        var result = await controller.GetAll();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(appointments, okResult.Value);
    }

    [Fact]
    public async Task Given_Valid_Id_When_GetById_Then_Returns_Ok()
    {
        // Arrange
        var builder = new Builder();
        var mockGetAppointmentByIdUseCase = new Mock<IGetAppointmentByIdUseCase>();
        var appointment = builder.Fixture.Create<AppointmentDto>();
        mockGetAppointmentByIdUseCase
            .Setup(useCase => useCase.ExecuteAsync(It.IsAny<int>()))
            .ReturnsAsync(appointment);

        var controller = builder.Build(
            new Mock<IGetAppointmentsUseCase>(),
            mockGetAppointmentByIdUseCase,
            new Mock<ICreateAppointmentUseCase>(),
            new Mock<IUpdateAppointmentUseCase>(),
            new Mock<IUpdateAppointmentStatusUseCase>(),
            new Mock<IDeleteAppointmentUseCase>()
        );

        // Act
        var result = await controller.GetById(builder.Fixture.Create<int>());

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(appointment, okResult.Value);
    }

    [Fact]
    public async Task Given_Valid_Input_When_Create_Then_Returns_Created()
    {
        // Arrange
        var builder = new Builder();
        var mockCreateAppointmentUseCase = new Mock<ICreateAppointmentUseCase>();

        var appointment = builder.Fixture.Create<AppointmentDto>();
        mockCreateAppointmentUseCase
            .Setup(useCase => useCase.ExecuteAsync(It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<string>()))
            .ReturnsAsync(appointment);

        var controller = builder.Build(
            new Mock<IGetAppointmentsUseCase>(),
            new Mock<IGetAppointmentByIdUseCase>(),
            mockCreateAppointmentUseCase,
            new Mock<IUpdateAppointmentUseCase>(),
            new Mock<IUpdateAppointmentStatusUseCase>(),
            new Mock<IDeleteAppointmentUseCase>()
        );

        // Act
        var result = await controller.Create(appointment.Title, appointment.StartTime, appointment.EndTime, appointment.Description);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(nameof(AppointmentsController.GetById), createdResult.ActionName);
        Assert.Equal(Builder.ControllerName, createdResult.ControllerName);
        Assert.Equal(appointment.Id, createdResult.RouteValues?[nameof(appointment.Id)]);
        Assert.Equal(appointment, createdResult.Value);
    }

    [Fact]
    public async Task Given_Valid_Id_And_Input_When_Update_Then_Returns_Ok()
    {
        // Arrange
        var builder = new Builder();
        var mockUpdateAppointmentUseCase = new Mock<IUpdateAppointmentUseCase>();

        var appointment = builder.Fixture.Create<AppointmentDto>();
        mockUpdateAppointmentUseCase
            .Setup(useCase => useCase.ExecuteAsync(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<string>(), It.IsAny<AppointmentStatus>()))
            .ReturnsAsync(appointment);

        var controller = builder.Build(
            new Mock<IGetAppointmentsUseCase>(),
            new Mock<IGetAppointmentByIdUseCase>(),
            new Mock<ICreateAppointmentUseCase>(),
            mockUpdateAppointmentUseCase,
            new Mock<IUpdateAppointmentStatusUseCase>(),
            new Mock<IDeleteAppointmentUseCase>()
        );

        // Act
        var result = await controller.Update(appointment.Id, appointment.Title, appointment.StartTime, appointment.EndTime, appointment.Description, AppointmentStatus.Scheduled);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(appointment, okResult.Value);
    }

    [Fact]
    public async Task Given_Valid_Id_And_Status_When_UpdateStatus_Then_Returns_Ok()
    {
        // Arrange
        var builder = new Builder();
        var mockUpdateAppointmentStatusUseCase = new Mock<IUpdateAppointmentStatusUseCase>();

        var appointment = builder.Fixture.Create<AppointmentDto>();
        mockUpdateAppointmentStatusUseCase
            .Setup(useCase => useCase.ExecuteAsync(It.IsAny<int>(), It.IsAny<AppointmentStatus>()))
            .ReturnsAsync(appointment);

        var controller = builder.Build(
            new Mock<IGetAppointmentsUseCase>(),
            new Mock<IGetAppointmentByIdUseCase>(),
            new Mock<ICreateAppointmentUseCase>(),
            new Mock<IUpdateAppointmentUseCase>(),
            mockUpdateAppointmentStatusUseCase,
            new Mock<IDeleteAppointmentUseCase>()
        );

        // Act
        var result = await controller.UpdateStatus(appointment.Id, AppointmentStatus.Completed);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(appointment, okResult.Value);
    }

    [Fact]
    public async Task Given_Valid_Id_When_Delete_Then_Returns_NoContent()
    {
        // Arrange
        var builder = new Builder();
        var mockDeleteAppointmentUseCase = new Mock<IDeleteAppointmentUseCase>();

        var controller = builder.Build(
            new Mock<IGetAppointmentsUseCase>(),
            new Mock<IGetAppointmentByIdUseCase>(),
            new Mock<ICreateAppointmentUseCase>(),
            new Mock<IUpdateAppointmentUseCase>(),
            new Mock<IUpdateAppointmentStatusUseCase>(),
            mockDeleteAppointmentUseCase
        );

        // Act
        var result = await controller.Delete(builder.Fixture.Create<int>());

        // Assert
        Assert.IsType<NoContentResult>(result);
    }
}