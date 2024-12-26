using AutoFixture;
using Microservice.Appointments.Application.EventHandlers;
using Microservice.Appointments.Application.Repositories;
using Microservice.Appointments.Domain.Enums;
using Microservice.Appointments.Domain.Events;
using Microservice.Appointments.Domain.Models;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Microservice.Appointments.UnitTests.Application.EventHandlers;

public class AppointmentNotificationHandlerTests
{
    private const string AppointmentReminderType = "AppointmentNotificationEvent";

    #region Builder

    private class Builder
    {
        public Fixture Fixture { get; } = new();
        public Mock<IAppointmentRepository> MockRepository { get; } = new();
        public Mock<ILogger<AppointmentNotificationHandler>> MockLogger { get; } = new();

        public AppointmentNotificationHandler Build()
        {
            return new AppointmentNotificationHandler(
                MockRepository.Object,
                MockLogger.Object);
        }

        public AppointmentDomain BuildDomain()
            => AppointmentDomain.Hydrate(
                Fixture.Create<int>(),
                Fixture.Create<string>(),
                DateTime.UtcNow.AddDays(-1),
                DateTime.UtcNow.AddDays(1),
                Fixture.Create<string>(),
                Fixture.Create<AppointmentStatus>()
            );

        public AppointmentNotificationEvent BuildEvent(string type, int appointmentId)
            => Fixture.Build<AppointmentNotificationEvent>()
                .With(e => e.Type, type)
                .With(e => e.AppointmentId, appointmentId)
                .Create();
    }

    #endregion Builder

    [Fact]
    public async Task Given_Event_Type_Is_Not_Appointment_Reminder_When_HandleAsync_Called_Then_Does_Nothing()
    {
        // Arrange
        var builder = new Builder();
        var handler = builder.Build();
        var appointmentNotificationEvent = builder.BuildEvent(builder.Fixture.Create<string>(), builder.Fixture.Create<int>());

        // Act
        await handler.HandleAsync(appointmentNotificationEvent);

        // Assert
        builder.MockRepository.Verify(repo => repo.GetAsync(It.IsAny<int>()), Times.Never);
        builder.MockLogger.Verify(logger =>
            logger.Log(It.IsAny<LogLevel>(), It.IsAny<EventId>(), It.IsAny<object>(), It.IsAny<Exception>(), It.IsAny<Func<object, Exception, string>>()), Times.Never);
    }

    [Fact]
    public async Task Given_Appointment_Not_Found_When_HandleAsync_Called_Then_Logs_Warning()
    {
        // Arrange
        var builder = new Builder();
        var handler = builder.Build();
        var appointmentNotificationEvent = builder.BuildEvent(AppointmentReminderType, builder.Fixture.Create<int>());

        builder.MockRepository
            .Setup(repo => repo.GetAsync(appointmentNotificationEvent.AppointmentId))
            .ReturnsAsync((AppointmentDomain)null!);

        // Act
        await handler.HandleAsync(appointmentNotificationEvent);

        // Assert
        builder.MockLogger.Verify(logger =>
            logger.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()!), Times.Once);
    }

    [Fact]
    public async Task Given_Appointment_Found_When_HandleAsync_Called_Then_Updates_Appointment_And_Logs_Info()
    {
        // Arrange
        var builder = new Builder();
        var handler = builder.Build();
        var appointment = builder.BuildDomain();
        var appointmentNotificationEvent = builder.BuildEvent(AppointmentReminderType, appointment.Id);

        builder.MockRepository
            .Setup(repo => repo.GetAsync(appointmentNotificationEvent.AppointmentId))
            .ReturnsAsync(appointment);

        builder.MockRepository
            .Setup(repo => repo.UpdateAsync(appointment))
            .ReturnsAsync(appointment);

        // Act
        await handler.HandleAsync(appointmentNotificationEvent);

        // Assert
        builder.MockRepository.Verify(repo => repo.UpdateAsync(appointment), Times.Once);
        builder.MockLogger.Verify(
            logger => logger.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                null,
                (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
            Times.Once);
    }

    [Fact]
    public async Task Given_Repository_Throws_Exception_When_HandleAsync_Called_Then_Throws_Exception()
    {
        // Arrange
        var builder = new Builder();
        var handler = builder.Build();
        var appointment = builder.BuildDomain();
        var appointmentNotificationEvent = builder.BuildEvent(AppointmentReminderType, appointment.Id);

        builder.MockRepository
            .Setup(repo => repo.GetAsync(appointmentNotificationEvent.AppointmentId))
            .ReturnsAsync(appointment);

        builder.MockRepository
            .Setup(repo => repo.UpdateAsync(appointment))
            .ThrowsAsync(new Exception(builder.Fixture.Create<string>()));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => handler.HandleAsync(appointmentNotificationEvent));
    }
}