using AutoFixture;
using Microservice.Appointments.Application.Dtos.Appointments;
using Microservice.Appointments.Application.Repositories;
using Microservice.Appointments.Application.UseCases;
using Microservice.Appointments.Application.UseCases.Mappers.Abstractions;
using Microservice.Appointments.Domain.Enums;
using Microservice.Appointments.Domain.Models;
using Moq;
using Xunit;

namespace Microservice.Appointments.UnitTests.Application.UseCases;

public class GetAppointmentsUseCaseTests
{
    private const int InitialValue = 0;
    private const int AppointmentCount = 3;

    #region Builder

    private class Builder
    {
        public Fixture Fixture { get; } = new Fixture();
        public Mock<IAppointmentRepository> MockRepository { get; } = new();
        public Mock<IAppointmentMapper> MockMapper { get; } = new();

        public GetAppointmentsUseCase Build()
        {
            return new GetAppointmentsUseCase(MockRepository.Object, MockMapper.Object);
        }

        public List<AppointmentDomain> BuildAppointmentDomains(int count)
        {
            return Enumerable
                    .Range(InitialValue, count)
                    .Select(_ => AppointmentDomain.Hydrate(
                        Fixture.Create<int>(),
                        Fixture.Create<string>(),
                        DateTime.UtcNow.AddDays(-1),
                        DateTime.UtcNow.AddDays(1),
                        Fixture.Create<string>(),
                        Fixture.Create<AppointmentStatus>()
                    ))
                    .ToList();
        }
    }

    #endregion Builder

    [Fact]
    public async Task Given_RepositoryReturnsAppointments_When_ExecuteAsync_Then_ReturnsMappedDtos()
    {
        // Arrange
        var builder = new Builder();
        var appointmentDomains = builder.BuildAppointmentDomains(AppointmentCount);
        var appointmentDtos = builder.Fixture.CreateMany<AppointmentDto>(appointmentDomains.Count).ToList();

        builder.MockRepository
            .Setup(repo => repo.GetAsync())
            .ReturnsAsync(appointmentDomains);

        for (var index = InitialValue; index < appointmentDomains.Count; index++)
        {
            var domain = appointmentDomains[index];
            var dto = appointmentDtos[index];

            builder.MockMapper
                .Setup(mapper => mapper.ToDto(domain))
                .Returns(dto);
        }

        var useCase = builder.Build();

        // Act
        var result = (await useCase.ExecuteAsync()).ToList();

        // Assert
        Assert.Equal(appointmentDtos.Count, result.Count);

        for (var index = InitialValue; index < appointmentDtos.Count; index++)
        {
            Assert.Equal(appointmentDtos[index], result[index]);
        }

        builder.MockRepository.Verify(repo => repo.GetAsync(), Times.Once);

        foreach (var domain in appointmentDomains)
        {
            builder.MockMapper.Verify(mapper => mapper.ToDto(domain), Times.Once);
        }
    }

    [Fact]
    public async Task Given_RepositoryReturnsEmptyList_When_ExecuteAsync_Then_ReturnsEmptyDtoList()
    {
        // Arrange
        var builder = new Builder();

        builder.MockRepository
            .Setup(repo => repo.GetAsync())
            .ReturnsAsync(new List<AppointmentDomain>());

        var useCase = builder.Build();

        // Act
        var result = await useCase.ExecuteAsync();

        // Assert
        Assert.Empty(result);
        builder.MockRepository.Verify(repo => repo.GetAsync(), Times.Once);
        builder.MockMapper.Verify(mapper => mapper.ToDto(It.IsAny<AppointmentDomain>()), Times.Never);
    }

    [Fact]
    public void Given_NullDependencies_When_Constructed_Then_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            new GetAppointmentsUseCase(null!, new Mock<IAppointmentMapper>().Object));

        Assert.Throws<ArgumentNullException>(() =>
            new GetAppointmentsUseCase(new Mock<IAppointmentRepository>().Object, null!));
    }
}