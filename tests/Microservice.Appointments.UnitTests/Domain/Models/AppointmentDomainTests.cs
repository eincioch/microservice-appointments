using AutoFixture;
using Microservice.Appointments.Domain.Enums;
using Microservice.Appointments.Domain.Exceptions;
using Microservice.Appointments.Domain.Models;
using Xunit;

namespace Microservice.Appointments.UnitTests.Domain.Models;

public class AppointmentDomainTests
{
    private const int InvalidId = 0;
    private const int NegativeId = -1;
    private const int ValidHoursToAdd = 1;
    private const int InvalidHoursToAdd = -1;

    private readonly Fixture _fixture = new();

    [Fact]
    public void Given_NullOrEmptyTitle_When_Constructed_Then_ThrowsDomainValidationException()
    {
        // Arrange
        var startTime = DateTime.UtcNow;
        var endTime = startTime.AddHours(ValidHoursToAdd);

        // Act & Assert
        Assert.Throws<DomainValidationException>(() =>
            new AppointmentDomain(null!, startTime, endTime, _fixture.Create<string>()));

        Assert.Throws<DomainValidationException>(() =>
            new AppointmentDomain(string.Empty, startTime, endTime, _fixture.Create<string>()));
    }

    [Fact]
    public void Given_InvalidDates_When_Constructed_Then_ThrowsDomainValidationException()
    {
        // Arrange
        var startTime = DateTime.UtcNow;
        var endTime = startTime.AddHours(InvalidHoursToAdd);

        // Act & Assert
        Assert.Throws<DomainValidationException>(() =>
            new AppointmentDomain(_fixture.Create<string>(), startTime, endTime, _fixture.Create<string>()));
    }

    [Fact]
    public void Given_ValidData_When_Constructed_Then_PropertiesAreSetCorrectly()
    {
        // Arrange
        var title = _fixture.Create<string>();
        var startTime = DateTime.UtcNow;
        var endTime = startTime.AddHours(ValidHoursToAdd);
        var description = _fixture.Create<string>();

        // Act
        var appointment = new AppointmentDomain(title, startTime, endTime, description);

        // Assert
        Assert.Equal(title, appointment.Title);
        Assert.Equal(startTime, appointment.StartTime);
        Assert.Equal(endTime, appointment.EndTime);
        Assert.Equal(description, appointment.Description);
        Assert.Equal(AppointmentStatus.Scheduled, appointment.Status);
    }

    [Fact]
    public void Given_InvalidId_When_AssignId_Then_ThrowsDomainValidationException()
    {
        // Arrange
        var appointment = CreateValidAppointmentDomain();

        // Act & Assert
        Assert.Throws<DomainValidationException>(() => appointment.AssignId(InvalidId));
        Assert.Throws<DomainValidationException>(() => appointment.AssignId(NegativeId));
    }

    [Fact]
    public void Given_IdAlreadyAssigned_When_AssignId_Then_ThrowsDomainValidationException()
    {
        // Arrange
        var appointment = CreateValidAppointmentDomain();
        var id = _fixture.Create<int>();
        appointment.AssignId(id);

        // Act & Assert
        Assert.Throws<DomainValidationException>(() => appointment.AssignId(_fixture.Create<int>()));
    }

    [Fact]
    public void Given_ValidId_When_AssignId_Then_IdIsAssigned()
    {
        // Arrange
        var appointment = CreateValidAppointmentDomain();
        var id = _fixture.Create<int>();

        // Act
        appointment.AssignId(id);

        // Assert
        Assert.Equal(id, appointment.Id);
    }

    [Fact]
    public void Given_StatusAlreadyCompleted_When_Complete_Then_ThrowsDomainValidationException()
    {
        // Arrange
        var appointment = CreateValidAppointmentDomain();
        appointment.Complete();

        // Act & Assert
        Assert.Throws<DomainValidationException>(() => appointment.Complete());
    }

    [Fact]
    public void Given_ValidStatus_When_Complete_Then_StatusIsUpdated()
    {
        // Arrange
        var appointment = CreateValidAppointmentDomain();

        // Act
        appointment.Complete();

        // Assert
        Assert.Equal(AppointmentStatus.Completed, appointment.Status);
    }

    [Fact]
    public void Given_StatusAlreadyCanceled_When_Cancel_Then_ThrowsDomainValidationException()
    {
        // Arrange
        var appointment = CreateValidAppointmentDomain();
        appointment.Cancel();

        // Act & Assert
        Assert.Throws<DomainValidationException>(() => appointment.Cancel());
    }

    [Fact]
    public void Given_ValidStatus_When_Cancel_Then_StatusIsUpdated()
    {
        // Arrange
        var appointment = CreateValidAppointmentDomain();

        // Act
        appointment.Cancel();

        // Assert
        Assert.Equal(AppointmentStatus.Canceled, appointment.Status);
    }

    [Fact]
    public void Given_ValidData_When_HydrateIsCalled_Then_PropertiesAreSetCorrectly()
    {
        // Arrange
        var id = _fixture.Create<int>();
        var title = _fixture.Create<string>();
        var startTime = DateTime.UtcNow;
        var endTime = startTime.AddHours(ValidHoursToAdd);
        var description = _fixture.Create<string>();
        var status = AppointmentStatus.Scheduled;

        // Act
        var appointment = AppointmentDomain.Hydrate(id, title, startTime, endTime, description, status);

        // Assert
        Assert.Equal(id, appointment.Id);
        Assert.Equal(title, appointment.Title);
        Assert.Equal(startTime, appointment.StartTime);
        Assert.Equal(endTime, appointment.EndTime);
        Assert.Equal(description, appointment.Description);
        Assert.Equal(status, appointment.Status);
    }

    private AppointmentDomain CreateValidAppointmentDomain()
    {
        return new AppointmentDomain(
            _fixture.Create<string>(),
            DateTime.UtcNow,
            DateTime.UtcNow.AddHours(ValidHoursToAdd),
            _fixture.Create<string>()
        );
    }
}
