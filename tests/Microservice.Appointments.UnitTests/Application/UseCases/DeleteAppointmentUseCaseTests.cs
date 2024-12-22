using AutoFixture;
using Microservice.Appointments.Application.Configuration;
using Microservice.Appointments.Application.Repositories;
using Microservice.Appointments.Application.UseCases;
using Microservice.Appointments.Application.UseCases.Mappers.Abstractions;
using Microservice.Appointments.Domain.Enums;
using Microservice.Appointments.Domain.Events;
using Microservice.Appointments.Domain.Exceptions;
using Microservice.Appointments.Domain.Models;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Microservice.Appointments.UnitTests.Application.UseCases;

public class DeleteAppointmentUseCaseTests
{
    private const string ValidationErrorMessage = "Validation error occurred while deleting the appointment.";

    #region Builder

    private class Builder
    {
        public Fixture Fixture { get; } = new();
        public Mock<IAppointmentRepository> MockRepository { get; } = new();
        public Mock<IAppointmentMapper> MockMapper { get; } = new();
        public Mock<IEventBus> MockEventBus { get; } = new();
        public Mock<ILogger<DeleteAppointmentUseCase>> MockLogger { get; } = new();

        public DeleteAppointmentUseCase Build()
        {
            return new DeleteAppointmentUseCase(
                MockRepository.Object,
                MockMapper.Object,
                MockEventBus.Object,
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
    }

    #endregion Builder

    [Fact]
    public async Task Given_ValidParameters_When_ExecuteAsync_Then_DeletesAppointmentSuccessfully()
    {
        // Arrange
        var builder = new Builder();
        var domainAppointment = builder.BuildDomain();
        var eventMessage = builder.Fixture.Create<AppointmentDeletedEvent>();

        builder.MockRepository
            .Setup(repo => repo.GetAsync(It.IsAny<int>()))
            .ReturnsAsync(domainAppointment);

        builder.MockMapper
            .Setup(mapper => mapper.ToDeletedMessage(It.IsAny<AppointmentDomain>()))
            .Returns(eventMessage);

        builder.MockEventBus
            .Setup(bus => bus.PublishAsync(It.IsAny<AppointmentDeletedEvent>(), It.IsAny<string>()))
            .Returns(Task.CompletedTask);

        var useCase = builder.Build();

        // Act
        await useCase.ExecuteAsync(domainAppointment.Id);

        // Assert
        builder.MockRepository.Verify(repo => repo.GetAsync(It.IsAny<int>()), Times.Once);
        builder.MockRepository.Verify(repo => repo.RemoveAsync(It.IsAny<AppointmentDomain>()), Times.Once);
        builder.MockMapper.Verify(mapper => mapper.ToDeletedMessage(It.IsAny<AppointmentDomain>()), Times.Once);
        builder.MockEventBus.Verify(bus => bus.PublishAsync(eventMessage, It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task Given_DomainValidationException_When_ExecuteAsync_Then_ThrowsBadRequestException()
    {
        // Arrange
        var builder = new Builder();
        var validationException = new DomainValidationException(builder.Fixture.Create<string>());

        builder.MockRepository
            .Setup(repo => repo.GetAsync(It.IsAny<int>()))
            .ReturnsAsync(builder.BuildDomain());

        builder.MockRepository
            .Setup(repo => repo.RemoveAsync(It.IsAny<AppointmentDomain>()))
            .Throws(validationException);

        var useCase = builder.Build();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<BadRequestException>(() =>
            useCase.ExecuteAsync(builder.Fixture.Create<int>()));

        Assert.Equal(ValidationErrorMessage, exception.Message);

        builder.MockLogger.Verify(logger =>
                logger.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains(ValidationErrorMessage)),
                    validationException,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()!),
            Times.Once);
    }

    [Fact]
    public async Task Given_AppointmentNotFound_When_ExecuteAsync_Then_ThrowsNotFoundException()
    {
        // Arrange
        var builder = new Builder();
        var appointmentId = builder.Fixture.Create<int>();

        builder.MockRepository
            .Setup(repo => repo.GetAsync(It.IsAny<int>()))!
            .ReturnsAsync((AppointmentDomain)null!);

        var useCase = builder.Build();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<NotFoundException>(() =>
            useCase.ExecuteAsync(appointmentId));

        Assert.Equal($"Appointment with id '{appointmentId}' not found.", exception.Message);

        builder.MockRepository.Verify(repo => repo.GetAsync(It.IsAny<int>()), Times.Once);
    }
}