using AutoFixture;
using Microservice.Appointments.Application.UseCases.Mappers;
using Microservice.Appointments.Domain.Enums;
using Microservice.Appointments.Domain.Models;
using Xunit;

namespace Microservice.Appointments.UnitTests.Application.UseCases.Mappers;

public class AppointmentMapperTests
{
    private readonly Fixture _fixture = new();

    private static AppointmentDomain CreateDomain(Fixture fixture)
    {
        return AppointmentDomain.Hydrate(fixture.Create<int>(), fixture.Create<string>(), DateTime.UtcNow.AddDays(-1), DateTime.UtcNow.AddDays(1), fixture.Create<string>(), fixture.Create<AppointmentStatus>());
    }

    [Fact]
    public void Given_AppointmentDomain_When_ToDtoIsCalled_Then_ReturnsCorrectAppointmentDto()
    {
        // Arrange
        var mapper = new AppointmentMapper();
        var domain = CreateDomain(_fixture);

        // Act
        var dto = mapper.ToDto(domain);

        // Assert
        Assert.Equal(domain.Id, dto.Id);
        Assert.Equal(domain.Title, dto.Title);
        Assert.Equal(domain.StartTime, dto.StartTime);
        Assert.Equal(domain.EndTime, dto.EndTime);
        Assert.Equal(domain.Description, dto.Description);
        Assert.Equal(domain.Status, dto.Status);
    }

    [Fact]
    public void Given_AppointmentDomain_When_ToAppointmentCreatedMessageIsCalled_Then_ReturnsCorrectEvent()
    {
        // Arrange
        var mapper = new AppointmentMapper();
        var domain = CreateDomain(_fixture);

        // Act
        var eventMessage = mapper.ToAppointmentCreatedMessage(domain);

        // Assert
        Assert.Equal(domain.Id, eventMessage.AppointmentId);
        Assert.Equal(domain.Title, eventMessage.Title);
        Assert.Equal(domain.StartTime, eventMessage.StartTime);
        Assert.Equal(domain.EndTime, eventMessage.EndTime);
        Assert.Equal(domain.Description, eventMessage.Description);
        Assert.Equal(domain.Status, eventMessage.Status);
    }
}