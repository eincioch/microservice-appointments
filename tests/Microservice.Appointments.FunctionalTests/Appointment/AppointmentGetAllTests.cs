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
    public async Task Given_Appointments_Exist_When_GetAll_Called_Then_Returns_Ok()
    {
        // Arrange
        var controller = CreateController();
        var appointmentDomains = CreateDomainList();
        var appointmentDtos = appointmentDomains
            .Select(domain => new AppointmentDto(
                domain.Id,
                domain.Title,
                domain.StartTime,
                domain.EndTime,
                domain.Description,
                domain.Status))
            .ToList();

        MockRepository
            .Setup(repo => repo.GetAsync())
            .ReturnsAsync(appointmentDomains);

        // Act
        var response = await controller.GetAll();

        // Assert
        var actionResult = Assert.IsType<ActionResult<IEnumerable<AppointmentDto>>>(response);
        var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
        var result = Assert.IsType<List<AppointmentDto>>(okResult.Value);

        Assert.Equal(appointmentDtos.Count, result.Count);
        Assert.True(appointmentDtos.SequenceEqual(result));
    }

    [Fact]
    public async Task Given_No_Appointments_Exist_When_GetAll_Called_Then_Returns_Empty_List()
    {
        // Arrange
        var controller = CreateController();

        MockRepository
            .Setup(repo => repo.GetAsync())
            .ReturnsAsync(new List<AppointmentDomain>());

        // Act
        var response = await controller.GetAll();

        // Assert
        var actionResult = Assert.IsType<ActionResult<IEnumerable<AppointmentDto>>>(response);
        var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
        var result = Assert.IsType<List<AppointmentDto>>(okResult.Value);

        Assert.Empty(result);
    }
}