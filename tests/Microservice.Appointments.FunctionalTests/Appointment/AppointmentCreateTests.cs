using AutoFixture;
using Microservice.Appointments.Application.Dtos.Appointments;
using Microservice.Appointments.Domain.Exceptions;
using Microservice.Appointments.Domain.Models;
using Microservice.Appointments.FunctionalTests.Appointment.Base;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Microservice.Appointments.FunctionalTests.Appointment;

public class AppointmentCreateTests : AppointmentTestsBase
{
    private const string ValidationErrorMessage = "Validation error occurred while creating an appointment.";
    private const int HoursInFuture = 1;

    [Fact]
    public async Task Create_ReturnsCreated_WhenAppointmentIsSuccessfullyCreated()
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
            .Setup(repo => repo.AddAsync(It.IsAny<AppointmentDomain>()))
            .ReturnsAsync(appointmentDomain);

        // Act
        var response = await controller.Create(
            appointmentDto.Title,
            appointmentDto.StartTime,
            appointmentDto.EndTime,
            appointmentDto.Description
        );

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(response);
        var result = Assert.IsType<AppointmentDto>(createdResult.Value);

        Assert.Equal(appointmentDto.Id, result.Id);
        Assert.Equal(appointmentDto.Title, result.Title);
        Assert.Equal(appointmentDto.StartTime, result.StartTime);
        Assert.Equal(appointmentDto.EndTime, result.EndTime);
        Assert.Equal(appointmentDto.Description, result.Description);
        Assert.Equal(appointmentDto.Status, result.Status);
    }

    [Fact]
    public async Task Create_ThrowsBadRequestException_WhenAppointmentIsInvalid()
    {
        // Arrange
        var controller = CreateController();
        var invalidTitle = string.Empty;
        var startTime = DateTime.UtcNow.AddHours(HoursInFuture);
        var endTime = DateTime.UtcNow;

        // Act & Assert
        var exception = await Assert.ThrowsAsync<BadRequestException>(() =>
            controller.Create(invalidTitle, startTime, endTime, Fixture.Create<string>()));

        Assert.Equal(ValidationErrorMessage, exception.Message);
    }
}