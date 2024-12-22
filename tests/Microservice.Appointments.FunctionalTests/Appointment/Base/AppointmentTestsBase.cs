using AutoFixture;
using Microservice.Appointments.Api.Controllers;
using Microservice.Appointments.Application.Configuration;
using Microservice.Appointments.Application.Repositories;
using Microservice.Appointments.Application.UseCases;
using Microservice.Appointments.Application.UseCases.Mappers;
using Microservice.Appointments.Domain.Enums;
using Microservice.Appointments.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace Microservice.Appointments.FunctionalTests.Appointment.Base;

public abstract class AppointmentTestsBase
{
    private const string ControllerName = "Appointments";
    private const int DaysInPast = -1;
    private const int DaysInFuture = 1;
    private const int InitialCount = 0;
    private const int DefaultCount = 3;

    protected const int InvalidId = -1;

    protected readonly Fixture Fixture = new();
    protected Mock<IAppointmentRepository> MockRepository { get; } = new();
    protected Mock<IEventBus> MockEventBus { get; } = new();

    protected AppointmentsController CreateController()
    {
        var mapper = new AppointmentMapper();
        var getAppointmentsUseCase = new GetAppointmentsUseCase(MockRepository.Object, mapper);
        var getAppointmentByIdUseCase = new GetAppointmentByIdUseCase(MockRepository.Object, mapper);
        var createAppointmentUseCase = new CreateAppointmentUseCase(
            MockRepository.Object,
            mapper,
            MockEventBus.Object,
            NullLogger<CreateAppointmentUseCase>.Instance
        );
        var updateAppointmentUseCase = new UpdateAppointmentUseCase(
            MockRepository.Object,
            mapper,
            MockEventBus.Object,
            NullLogger<UpdateAppointmentUseCase>.Instance
        );

        return new AppointmentsController(
            getAppointmentsUseCase,
            getAppointmentByIdUseCase,
            createAppointmentUseCase,
            updateAppointmentUseCase
        )
        {
            ControllerContext = new ControllerContext
            {
                ActionDescriptor = new Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor
                {
                    ControllerName = ControllerName
                }
            }
        };
    }

    protected List<AppointmentDomain> CreateDomainList(int count = DefaultCount)
        => Enumerable.Range(InitialCount, count)
            .Select(_ => CreateDomain())
            .ToList();

    protected AppointmentDomain CreateDomain()
        => AppointmentDomain.Hydrate(
            Fixture.Create<int>(),
            Fixture.Create<string>(),
            DateTime.UtcNow.AddDays(DaysInPast),
            DateTime.UtcNow.AddDays(DaysInFuture),
            Fixture.Create<string>(),
            Fixture.Create<AppointmentStatus>()
        );
}