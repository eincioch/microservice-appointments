using AutoFixture;
using Microservice.Appointments.Api.Requests;
using Microservice.Appointments.Application.Dtos.Appointments;
using Microservice.Appointments.Domain.Enums;
using Microservice.Appointments.Domain.Exceptions;
using Microservice.Appointments.Domain.Models;
using Microservice.Appointments.FunctionalTests.Appointment.Base;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Microservice.Appointments.FunctionalTests.Appointment;

public class AppointmentUpdateTests : AppointmentTestsBase
{
    private const string ValidationErrorMessage = "Validation error occurred while updating an appointment.";
    private const int HoursInFuture = 1;

    [Fact]
    public async Task Given_Valid_Appointment_When_Update_Called_Then_Returns_Ok()
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

        var updateRequest = new UpdateAppointmentRequest(
            appointmentDto.Title,
            appointmentDto.StartTime,
            appointmentDto.EndTime,
            appointmentDto.Description,
            appointmentDto.Status
        );

        MockRepository
            .Setup(repo => repo.GetAsync(It.IsAny<int>()))
            .ReturnsAsync(appointmentDomain);

        MockRepository
            .Setup(repo => repo.UpdateAsync(It.IsAny<AppointmentDomain>()))
            .ReturnsAsync(appointmentDomain);

        // Act
        var response = await controller.Update(appointmentDto.Id, updateRequest);

        // Assert
        var actionResult = Assert.IsType<ActionResult<AppointmentDto>>(response);
        var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
        var result = Assert.IsType<AppointmentDto>(okResult.Value);

        Assert.Equal(appointmentDto.Id, result.Id);
        Assert.Equal(appointmentDto.Title, result.Title);
        Assert.Equal(appointmentDto.StartTime, result.StartTime);
        Assert.Equal(appointmentDto.EndTime, result.EndTime);
        Assert.Equal(appointmentDto.Description, result.Description);
        Assert.Equal(appointmentDto.Status, result.Status);
    }

    [Fact]
    public async Task Given_Non_Existent_Appointment_When_Update_Called_Then_Throws_NotFoundException()
    {
        // Arrange
        var controller = CreateController();
        var updateRequest = new UpdateAppointmentRequest(
            string.Empty,
            DateTime.UtcNow.AddHours(HoursInFuture),
            DateTime.UtcNow,
            Fixture.Create<string>(),
            AppointmentStatus.Scheduled
        );

        var nonExistentId = Fixture.Create<int>();

        MockRepository
            .Setup(repo => repo.GetAsync(nonExistentId))
            .ReturnsAsync((AppointmentDomain)null!);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<NotFoundException>(() =>
            controller.Update(nonExistentId, updateRequest));

        Assert.Equal($"Appointment with id '{nonExistentId}' was not found.", exception.Message);
    }

    [Fact]
    public async Task Given_Invalid_Appointment_When_Update_Called_Then_Throws_BadRequestException()
    {
        // Arrange
        var controller = CreateController();
        var updateRequest = new UpdateAppointmentRequest(
            string.Empty,
            DateTime.UtcNow.AddHours(HoursInFuture),
            DateTime.UtcNow,
            Fixture.Create<string>(),
            AppointmentStatus.Scheduled
        );

        var appointmentDomain = CreateDomain();

        MockRepository
            .Setup(repo => repo.GetAsync(It.IsAny<int>()))
            .ReturnsAsync(appointmentDomain);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<BadRequestException>(() =>
            controller.Update(InvalidId, updateRequest));

        Assert.Equal(ValidationErrorMessage, exception.Message);
    }
}