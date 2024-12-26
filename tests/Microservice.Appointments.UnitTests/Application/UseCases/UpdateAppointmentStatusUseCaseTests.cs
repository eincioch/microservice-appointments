using AutoFixture;
using Microservice.Appointments.Application.Dtos.Appointments;
using Microservice.Appointments.Application.EventBus;
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

public class UpdateAppointmentStatusUseCaseTests
{
    private const string ValidationErrorMessage = "Validation error occurred while updating the appointment status.";
    private const string EventBusFailureMessage = "Event bus failure";

    #region Builder

    private class Builder
    {
        public Fixture Fixture { get; } = new();
        public Mock<IAppointmentRepository> MockRepository { get; } = new();
        public Mock<IAppointmentMapper> MockMapper { get; } = new();
        public Mock<IEventBus> MockEventBus { get; } = new();
        public Mock<ILogger<UpdateAppointmentStatusUseCase>> MockLogger { get; } = new();

        public UpdateAppointmentStatusUseCase Build()
        {
            return new UpdateAppointmentStatusUseCase(
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
    public async Task Given_Valid_Parameters_When_ExecuteAsync_Called_Then_Updates_Appointment_Status_Successfully()
    {
        // Arrange
        var builder = new Builder();
        var domainAppointment = builder.BuildDomain();
        var updatedEntity = builder.BuildDomain();
        var appointmentDto = builder.Fixture.Create<AppointmentDto>();
        var eventMessage = builder.Fixture.Create<AppointmentChangedEvent>();

        builder.MockRepository
            .Setup(repo => repo.GetAsync(It.IsAny<int>()))
            .ReturnsAsync(domainAppointment);

        builder.MockRepository
            .Setup(repo => repo.UpdateAsync(It.IsAny<AppointmentDomain>()))
            .ReturnsAsync(updatedEntity);

        builder.MockMapper
            .Setup(mapper => mapper.ToChangedMessage(It.IsAny<AppointmentDomain>()))
            .Returns(eventMessage);

        builder.MockMapper
            .Setup(mapper => mapper.ToDto(It.IsAny<AppointmentDomain>()))
            .Returns(appointmentDto);

        builder.MockEventBus
            .Setup(bus => bus.PublishAsync(It.IsAny<AppointmentChangedEvent>(), It.IsAny<string>()))
            .Returns(Task.CompletedTask);

        var useCase = builder.Build();

        // Act
        var result = await useCase.ExecuteAsync(domainAppointment.Id, domainAppointment.Status);

        // Assert
        Assert.Equal(appointmentDto, result);

        builder.MockRepository.Verify(repo => repo.GetAsync(It.IsAny<int>()), Times.Once);
        builder.MockRepository.Verify(repo => repo.UpdateAsync(It.IsAny<AppointmentDomain>()), Times.Once);
        builder.MockMapper.Verify(mapper => mapper.ToDto(It.IsAny<AppointmentDomain>()), Times.Once);
        builder.MockEventBus.Verify(bus => bus.PublishAsync(eventMessage, It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task Given_DomainValidationException_When_ExecuteAsync_Called_Then_Throws_BadRequestException()
    {
        // Arrange
        var builder = new Builder();
        var validationException = new DomainValidationException(builder.Fixture.Create<string>());

        builder.MockRepository
            .Setup(repo => repo.GetAsync(It.IsAny<int>()))
            .ReturnsAsync(builder.BuildDomain());

        builder.MockRepository
            .Setup(repo => repo.UpdateAsync(It.IsAny<AppointmentDomain>()))
            .Throws(validationException);

        var useCase = builder.Build();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<BadRequestException>(() =>
            useCase.ExecuteAsync(builder.Fixture.Create<int>(), builder.Fixture.Create<AppointmentStatus>()));

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
    public async Task Given_Event_Bus_Error_When_ExecuteAsync_Called_Then_Throws_Exception()
    {
        // Arrange
        var builder = new Builder();
        var updatedEntity = builder.BuildDomain();
        var eventBusException = new Exception(EventBusFailureMessage);

        builder.MockRepository
            .Setup(repo => repo.GetAsync(It.IsAny<int>()))
            .ReturnsAsync(builder.BuildDomain());

        builder.MockRepository
            .Setup(repo => repo.UpdateAsync(It.IsAny<AppointmentDomain>()))
            .ReturnsAsync(updatedEntity);

        builder.MockEventBus
            .Setup(bus => bus.PublishAsync(It.IsAny<AppointmentChangedEvent>(), It.IsAny<string>()))
            .Throws(eventBusException);

        var useCase = builder.Build();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(() =>
            useCase.ExecuteAsync(builder.Fixture.Create<int>(), builder.Fixture.Create<AppointmentStatus>()));

        Assert.Equal(EventBusFailureMessage, exception.Message);
    }

    [Fact]
    public void Given_Null_Dependencies_When_Constructed_Then_Throws_ArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            new UpdateAppointmentStatusUseCase(null!, new Mock<IAppointmentMapper>().Object, new Mock<IEventBus>().Object, new Mock<ILogger<UpdateAppointmentStatusUseCase>>().Object));

        Assert.Throws<ArgumentNullException>(() =>
            new UpdateAppointmentStatusUseCase(new Mock<IAppointmentRepository>().Object, null!, new Mock<IEventBus>().Object, new Mock<ILogger<UpdateAppointmentStatusUseCase>>().Object));

        Assert.Throws<ArgumentNullException>(() =>
            new UpdateAppointmentStatusUseCase(new Mock<IAppointmentRepository>().Object, new Mock<IAppointmentMapper>().Object, null!, new Mock<ILogger<UpdateAppointmentStatusUseCase>>().Object));

        Assert.Throws<ArgumentNullException>(() =>
            new UpdateAppointmentStatusUseCase(new Mock<IAppointmentRepository>().Object, new Mock<IAppointmentMapper>().Object, new Mock<IEventBus>().Object, null!));
    }

    [Fact]
    public async Task Given_Appointment_Not_Found_When_ExecuteAsync_Called_Then_Throws_NotFoundException()
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
            useCase.ExecuteAsync(appointmentId, builder.Fixture.Create<AppointmentStatus>()));

        Assert.Equal($"Appointment with id '{appointmentId}' was not found.", exception.Message);

        builder.MockRepository.Verify(repo => repo.GetAsync(It.IsAny<int>()), Times.Once);
    }
}