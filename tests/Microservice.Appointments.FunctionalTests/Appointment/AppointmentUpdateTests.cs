using AutoFixture;
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
    public async Task Update_ReturnsOk_WhenAppointmentIsSuccessfullyUpdated()
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
            .Setup(repo => repo.GetAsync(It.IsAny<int>()))
            .ReturnsAsync(appointmentDomain);

        MockRepository
            .Setup(repo => repo.UpdateAsync(It.IsAny<AppointmentDomain>()))
            .ReturnsAsync(appointmentDomain);

        // Act
        var response = await controller.Update(
            appointmentDto.Id,
            appointmentDto.Title,
            appointmentDto.StartTime,
            appointmentDto.EndTime,
            appointmentDto.Description,
            appointmentDto.Status
        );

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
    public async Task Update_ThrowsNotFoundException_WhenAppointmentNotExists()
    {
        // Arrange
        var controller = CreateController();
        var invalidTitle = string.Empty;
        var startTime = DateTime.UtcNow.AddHours(HoursInFuture);
        var endTime = DateTime.UtcNow;
        var nonExistentId = Fixture.Create<int>();

        MockRepository
            .Setup(repo => repo.GetAsync(nonExistentId))!
            .ReturnsAsync((AppointmentDomain)null!);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<NotFoundException>(() =>
            controller.Update(nonExistentId, invalidTitle, startTime, endTime, Fixture.Create<string>(), AppointmentStatus.Scheduled));

        Assert.Equal($"Appointment with id '{nonExistentId}' was not found.", exception.Message);
    }

    [Fact]
    public async Task Update_ThrowsBadRequestException_WhenAppointmentIsInvalid()
    {
        // Arrange
        var controller = CreateController();
        var invalidTitle = string.Empty;
        var startTime = DateTime.UtcNow.AddHours(HoursInFuture);
        var endTime = DateTime.UtcNow;
        var appointmentDomain = CreateDomain();

        MockRepository
            .Setup(repo => repo.GetAsync(It.IsAny<int>()))
            .ReturnsAsync(appointmentDomain);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<BadRequestException>(() =>
            controller.Update(InvalidId, invalidTitle, startTime, endTime, Fixture.Create<string>(), AppointmentStatus.Scheduled));

        Assert.Equal(ValidationErrorMessage, exception.Message);
    }
}