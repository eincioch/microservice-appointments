using AutoFixture;
using Microservice.Appointments.Application.EventHandlers;
using Microservice.Appointments.Application.Repositories;
using Microservice.Appointments.Domain.Enums;
using Microservice.Appointments.Domain.Events;
using Microservice.Appointments.Domain.Models;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Microservice.Appointments.FunctionalTests.Appointment.EventHandlers;

public class AppointmentNotificationEventHandlerTests
{
    private const string NotificationEventType = "AppointmentNotificationEvent";

    #region Builder

    private class Builder
    {
        public Fixture Fixture { get; } = new();
        public Mock<IAppointmentRepository> MockRepository { get; } = new();
        public Mock<ILogger<AppointmentNotificationHandler>> MockLogger { get; } = new();

        public AppointmentNotificationHandler Build()
        {
            return new AppointmentNotificationHandler(MockRepository.Object, MockLogger.Object);
        }

        public AppointmentDomain BuildDomain()
            => AppointmentDomain.Hydrate(
                Fixture.Create<int>(),
                Fixture.Create<string>(),
                Fixture.Create<DateTime>(),
                Fixture.Create<DateTime>(),
                Fixture.Create<string>(),
                Fixture.Create<AppointmentStatus>()
            );
    }

    #endregion Builder

    [Fact]
    public async Task HandleAsync_ProcessesNotification_WhenAppointmentExists()
    {
        // Arrange
        var builder = new Builder();
        var appointmentDomain = builder.BuildDomain();
        var appointmentNotificationEvent = new AppointmentNotificationEvent(
            builder.Fixture.Create<string>(),
            appointmentDomain.Id,
            NotificationEventType,
            builder.Fixture.Create<string>(),
            builder.Fixture.Create<DateTime>()
        );

        builder.MockRepository
            .Setup(repo => repo.GetAsync(It.IsAny<int>()))
            .ReturnsAsync(appointmentDomain);

        builder.MockRepository
            .Setup(repo => repo.UpdateAsync(It.IsAny<AppointmentDomain>()))
            .ReturnsAsync(appointmentDomain);

        var handler = builder.Build();

        // Act
        await handler.HandleAsync(appointmentNotificationEvent);

        // Assert
        builder.MockRepository.Verify(repo => repo.GetAsync(appointmentDomain.Id), Times.Once);
        builder.MockRepository.Verify(repo => repo.UpdateAsync(appointmentDomain), Times.Once);
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
    public async Task HandleAsync_DoesNotProcessNotification_WhenAppointmentDoesNotExist()
    {
        // Arrange
        var builder = new Builder();
        var appointmentNotificationEvent = new AppointmentNotificationEvent(
            builder.Fixture.Create<string>(),
            builder.Fixture.Create<int>(),
            NotificationEventType,
            builder.Fixture.Create<string>(),
            builder.Fixture.Create<DateTime>()
        );

        builder.MockRepository
            .Setup(repo => repo.GetAsync(It.IsAny<int>()))
            .ReturnsAsync((AppointmentDomain)null!);

        var handler = builder.Build();

        // Act
        await handler.HandleAsync(appointmentNotificationEvent);

        // Assert
        builder.MockRepository.Verify(repo => repo.GetAsync(It.IsAny<int>()), Times.Once);
        builder.MockRepository.Verify(repo => repo.UpdateAsync(It.IsAny<AppointmentDomain>()), Times.Never);
        builder.MockLogger.Verify(
            logger => logger.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                null,
                (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
            Times.Once);
    }

    [Fact]
    public async Task HandleAsync_DoesNotProcessNotification_WhenEventTypeIsNotAppointmentReminder()
    {
        // Arrange
        var builder = new Builder();
        var appointmentNotificationEvent = new AppointmentNotificationEvent(
            builder.Fixture.Create<string>(),
            builder.Fixture.Create<int>(),
            builder.Fixture.Create<string>(),
            builder.Fixture.Create<string>(),
            builder.Fixture.Create<DateTime>()
        );

        var handler = builder.Build();

        // Act
        await handler.HandleAsync(appointmentNotificationEvent);

        // Assert
        builder.MockRepository.Verify(repo => repo.GetAsync(It.IsAny<int>()), Times.Never);
        builder.MockRepository.Verify(repo => repo.UpdateAsync(It.IsAny<AppointmentDomain>()), Times.Never);
        builder.MockLogger.Verify(
            x => x.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
            Times.Never
        );
    }
}