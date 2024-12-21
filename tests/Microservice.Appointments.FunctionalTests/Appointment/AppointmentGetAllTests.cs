using Microservice.Appointments.Application.Dtos.Appointments;
using Microservice.Appointments.Domain.Models;
using Microservice.Appointments.FunctionalTests.Appointment.Base;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Microservice.Appointments.FunctionalTests.Appointment;

public class AppointmentGetAllTests : AppointmentTestsBase
{
    [Fact]
    public async Task GetAll_ReturnsOk_WhenAppointmentsExist()
    {
        // Arrange
        var controller = CreateController();
        var appointmentDomains = CreateDomainList();
        var appointmentDtos = appointmentDomains
            .Select(domain => new AppointmentDto(domain.Id, domain.Title, domain.StartTime, domain.EndTime, domain.Description, domain.Status))
            .ToList();

        MockRepository
            .Setup(repo => repo.GetAsync())
            .ReturnsAsync(appointmentDomains);

        // Act
        var response = await controller.GetAll();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(response);
        var result = Assert.IsType<List<AppointmentDto>>(okResult.Value);
        Assert.Equal(appointmentDtos.Count, result.Count);
    }

    [Fact]
    public async Task GetAll_ReturnsEmptyList_WhenNoAppointmentsExist()
    {
        // Arrange
        var controller = CreateController();

        MockRepository
            .Setup(repo => repo.GetAsync())
            .ReturnsAsync(new List<AppointmentDomain>());

        // Act
        var response = await controller.GetAll();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(response);
        var result = Assert.IsType<List<AppointmentDto>>(okResult.Value);
        Assert.Empty(result);
    }
}