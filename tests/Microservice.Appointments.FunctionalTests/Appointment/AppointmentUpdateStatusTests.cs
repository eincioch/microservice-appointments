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

public class AppointmentUpdateStatusTests : AppointmentTestsBase
{
    private const string ValidationErrorMessage = "Validation error occurred while updating the appointment status.";

    [Fact]
    public async Task UpdateStatus_ReturnsOk_WhenStatusIsSuccessfullyUpdated()
    {
        // Arrange
        var controller = CreateController();
        var appointmentDomain = CreateDomain();
        var newStatus = AppointmentStatus.Completed;

        MockRepository
            .Setup(repo => repo.GetAsync(It.IsAny<int>()))
            .ReturnsAsync(appointmentDomain);

        MockRepository
            .Setup(repo => repo.UpdateAsync(It.IsAny<AppointmentDomain>()))
            .ReturnsAsync(appointmentDomain);

        // Act
        var response = await controller.UpdateStatus(appointmentDomain.Id, newStatus);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(response);
        var result = Assert.IsType<AppointmentDto>(okResult.Value);

        Assert.Equal(appointmentDomain.Id, result.Id);
        Assert.Equal(newStatus, result.Status);
    }

    [Fact]
    public async Task UpdateStatus_ThrowsNotFoundException_WhenAppointmentNotExists()
    {
        // Arrange
        var controller = CreateController();
        var nonExistentId = Fixture.Create<int>();
        var newStatus = AppointmentStatus.Completed;

        MockRepository
            .Setup(repo => repo.GetAsync(nonExistentId))
            .ReturnsAsync((AppointmentDomain)null!);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<NotFoundException>(() =>
            controller.UpdateStatus(nonExistentId, newStatus));

        Assert.Equal($"Appointment with id '{nonExistentId}' was not found.", exception.Message);
    }

    [Fact]
    public async Task UpdateStatus_ThrowsBadRequestException_WhenStatusIsInvalid()
    {
        // Arrange
        var controller = CreateController();
        var appointmentDomain = CreateDomain();
        var invalidStatus = (AppointmentStatus)999;

        MockRepository
            .Setup(repo => repo.GetAsync(It.IsAny<int>()))
            .ReturnsAsync(appointmentDomain);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<BadRequestException>(() =>
            controller.UpdateStatus(appointmentDomain.Id, invalidStatus));

        Assert.Equal(ValidationErrorMessage, exception.Message);
    }
}
