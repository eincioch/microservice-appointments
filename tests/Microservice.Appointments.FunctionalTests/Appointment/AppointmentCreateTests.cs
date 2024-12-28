using AutoFixture;
using Microservice.Appointments.Api.Requests;
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
    public async Task Given_Valid_Appointment_When_Create_Called_Then_Returns_Created()
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

        var createRequest = new CreateAppointmentRequest(
            appointmentDto.Title,
            appointmentDto.StartTime,
            appointmentDto.EndTime,
            appointmentDto.Description
        );

        MockRepository
            .Setup(repo => repo.AddAsync(It.IsAny<AppointmentDomain>()))
            .ReturnsAsync(appointmentDomain);

        // Act
        var response = await controller.Create(createRequest);

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
    public async Task Given_Invalid_Appointment_When_Create_Called_Then_Throws_BadRequestException()
    {
        // Arrange
        var controller = CreateController();
        var invalidRequest = new CreateAppointmentRequest(
            string.Empty,
            DateTime.UtcNow.AddHours(HoursInFuture),
            DateTime.UtcNow,
            Fixture.Create<string>()
        );

        // Act & Assert
        var exception = await Assert.ThrowsAsync<BadRequestException>(() =>
            controller.Create(invalidRequest));

        Assert.Equal(ValidationErrorMessage, exception.Message);
    }
}