using AutoFixture;
using Microservice.Appointments.Application.Dtos.Appointments;
using Microservice.Appointments.Domain.Exceptions;
using Microservice.Appointments.Domain.Models;
using Microservice.Appointments.FunctionalTests.Appointment.Base;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Microservice.Appointments.FunctionalTests.Appointment;

public class AppointmentGetByIdTests : AppointmentTestsBase
{
    [Fact]
    public async Task Given_Valid_Id_When_GetById_Called_Then_Returns_Ok()
    {
        // Arrange
        var controller = CreateController();
        var appointmentDomain = CreateDomain();
        var appointmentDto = new AppointmentDto(
            appointmentDomain.Id,
            appointmentDomain.Title,
            appointmentDomain.StartTime,
            appointmentDomain.EndTime,
            appointmentDomain.Description,
            appointmentDomain.Status
        );

        MockRepository
            .Setup(repo => repo.GetAsync(appointmentDomain.Id))
            .ReturnsAsync(appointmentDomain);

        // Act
        var response = await controller.GetById(appointmentDomain.Id);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(response);
        var result = Assert.IsType<AppointmentDto>(okResult.Value);
        Assert.Equal(appointmentDto.Id, result.Id);
        Assert.Equal(appointmentDto.Title, result.Title);
        Assert.Equal(appointmentDto.StartTime, result.StartTime);
        Assert.Equal(appointmentDto.EndTime, result.EndTime);
        Assert.Equal(appointmentDto.Description, result.Description);
        Assert.Equal(appointmentDto.Status, result.Status);
    }

    [Fact]
    public async Task Given_Non_Existent_Id_When_GetById_Called_Then_Throws_NotFoundException()
    {
        // Arrange
        var controller = CreateController();
        var nonExistentId = Fixture.Create<int>();

        MockRepository
            .Setup(repo => repo.GetAsync(nonExistentId))!
            .ReturnsAsync((AppointmentDomain)null!);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<NotFoundException>(() =>
            controller.GetById(nonExistentId));

        Assert.Equal($"Appointment with id '{nonExistentId}' was not found.", exception.Message);
    }

    [Fact]
    public async Task Given_Invalid_Id_When_GetById_Called_Then_Throws_BadRequestException()
    {
        // Arrange
        var controller = CreateController();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<BadRequestException>(() =>
            controller.GetById(InvalidId));

        Assert.Equal($"Appointment with id '{InvalidId}' is invalid.", exception.Message);
    }
}