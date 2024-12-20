using Microservice.Appointments.Application.Dtos.Appointments;
using Microservice.Appointments.Application.Repositories;
using Microservice.Appointments.Application.UseCases.Abstractions;
using Microservice.Appointments.Application.UseCases.Mappers.Abstractions;

namespace Microservice.Appointments.Application.UseCases;

public class GetAppointmentsUseCase(IAppointmentRepository appointmentRepository, IAppointmentMapper appointmentMapper) : IGetAppointmentsUseCase
{
    private readonly IAppointmentRepository _appointmentRepository = appointmentRepository;
    private readonly IAppointmentMapper _appointmentMapper = appointmentMapper;

    public async Task<IEnumerable<AppointmentDto>> ExecuteAsync()
    {
        var appointments = await _appointmentRepository.GetAsync();

        var appointmentDtos = appointments
            .Select(x => _appointmentMapper.ToDto(x))
            .ToList();

        return appointmentDtos;
    }
}