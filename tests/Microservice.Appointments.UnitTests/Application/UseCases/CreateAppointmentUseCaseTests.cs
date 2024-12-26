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

public class CreateAppointmentUseCaseTests
{
    private const string ValidationErrorMessage = "Validation error occurred while creating an appointment.";
    private const string EventBusFailureMessage = "Event bus failure";
    private const int DaysInPast = -1;
    private const int DaysInFuture = 1;
    private const int HoursToAdd = 1;

    #region Builder

    private class Builder
    {
        public Fixture Fixture { get; } = new Fixture();
        public Mock<IAppointmentRepository> MockRepository { get; } = new();
        public Mock<IAppointmentMapper> MockMapper { get; } = new();
        public Mock<IEventBus> MockEventBus { get; } = new();
        public Mock<ILogger<CreateAppointmentUseCase>> MockLogger { get; } = new();

        public CreateAppointmentUseCase Build()
        {
            return new CreateAppointmentUseCase(
                MockRepository.Object,
                MockMapper.Object,
                MockEventBus.Object,
                MockLogger.Object);
        }

        public AppointmentDomain BuildDomain()
            => AppointmentDomain.Hydrate(
                Fixture.Create<int>(),
                Fixture.Create<string>(),
                DateTime.UtcNow.AddDays(DaysInPast),
                DateTime.UtcNow.AddDays(DaysInFuture),
                Fixture.Create<string>(),
                Fixture.Create<AppointmentStatus>()
            );
    }

    #endregion Builder

    [Fact]
    public async Task Given_Valid_Parameters_When_ExecuteAsync_Called_Then_Creates_Appointment_Successfully()
    {
        // Arrange
        var builder = new Builder();
        var domainAppointment = builder.BuildDomain();
        var savedEntity = builder.BuildDomain();
        var appointmentDto = builder.Fixture.Create<AppointmentDto>();
        var eventMessage = builder.Fixture.Create<AppointmentCreatedEvent>();

        builder.MockRepository
            .Setup(repo => repo.AddAsync(It.IsAny<AppointmentDomain>()))
            .ReturnsAsync(savedEntity);

        builder.MockMapper
            .Setup(mapper => mapper.ToCreatedMessage(It.IsAny<AppointmentDomain>()))
            .Returns(eventMessage);

        builder.MockMapper
            .Setup(mapper => mapper.ToDto(It.IsAny<AppointmentDomain>()))
            .Returns(appointmentDto);

        builder.MockEventBus
            .Setup(bus => bus.PublishAsync(It.IsAny<AppointmentCreatedEvent>(), It.IsAny<string>()))
            .Returns(Task.CompletedTask);

        var useCase = builder.Build();

        // Act
        var result = await useCase.ExecuteAsync(domainAppointment.Title, domainAppointment.StartTime, domainAppointment.EndTime, domainAppointment.Description);

        // Assert
        Assert.Equal(appointmentDto, result);

        builder.MockRepository.Verify(repo => repo.AddAsync(It.IsAny<AppointmentDomain>()), Times.Once);
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
            .Setup(repo => repo.AddAsync(It.IsAny<AppointmentDomain>()))
            .Throws(validationException);

        var useCase = builder.Build();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<BadRequestException>(() =>
            useCase.ExecuteAsync(builder.Fixture.Create<string>(), DateTime.UtcNow, DateTime.UtcNow.AddHours(HoursToAdd), builder.Fixture.Create<string>()));

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
        var savedEntity = builder.BuildDomain();
        var eventBusException = new Exception(EventBusFailureMessage);

        builder.MockRepository
            .Setup(repo => repo.AddAsync(It.IsAny<AppointmentDomain>()))
            .ReturnsAsync(savedEntity);

        builder.MockEventBus
            .Setup(bus => bus.PublishAsync(It.IsAny<AppointmentCreatedEvent>(), It.IsAny<string>()))
            .Throws(eventBusException);

        var useCase = builder.Build();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(() =>
            useCase.ExecuteAsync(builder.Fixture.Create<string>(), DateTime.UtcNow, DateTime.UtcNow.AddHours(HoursToAdd), builder.Fixture.Create<string>()));

        Assert.Equal(EventBusFailureMessage, exception.Message);
    }

    [Fact]
    public void Given_Null_Dependencies_When_Constructed_Then_Throws_ArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            new CreateAppointmentUseCase(null!, new Mock<IAppointmentMapper>().Object, new Mock<IEventBus>().Object, new Mock<ILogger<CreateAppointmentUseCase>>().Object));

        Assert.Throws<ArgumentNullException>(() =>
            new CreateAppointmentUseCase(new Mock<IAppointmentRepository>().Object, null!, new Mock<IEventBus>().Object, new Mock<ILogger<CreateAppointmentUseCase>>().Object));

        Assert.Throws<ArgumentNullException>(() =>
            new CreateAppointmentUseCase(new Mock<IAppointmentRepository>().Object, new Mock<IAppointmentMapper>().Object, null!, new Mock<ILogger<CreateAppointmentUseCase>>().Object));

        Assert.Throws<ArgumentNullException>(() =>
            new CreateAppointmentUseCase(new Mock<IAppointmentRepository>().Object, new Mock<IAppointmentMapper>().Object, new Mock<IEventBus>().Object, null!));
    }
}