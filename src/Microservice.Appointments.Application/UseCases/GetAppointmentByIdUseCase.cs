using Microservice.Appointments.Application.Dtos.Appointments;
using Microservice.Appointments.Application.Repositories;
using Microservice.Appointments.Application.UseCases.Abstractions;
using Microservice.Appointments.Application.UseCases.Mappers.Abstractions;
using Microservice.Appointments.Domain.Exceptions;

namespace Microservice.Appointments.Application.UseCases;

public class GetAppointmentByIdUseCase(IAppointmentRepository appointmentRepository, IAppointmentMapper appointmentMapper) : IGetAppointmentByIdUseCase
{
    private readonly IAppointmentRepository _appointmentRepository = appointmentRepository ?? throw new ArgumentNullException(nameof(appointmentRepository));
    private readonly IAppointmentMapper _appointmentMapper = appointmentMapper ?? throw new ArgumentNullException(nameof(appointmentMapper));

    private const int MinimumAllowedId = 0;

    public async Task<AppointmentDto> ExecuteAsync(int id)
    {
        if (id <= MinimumAllowedId)
            throw new BadRequestException($"Appointment with id '{id}' is invalid.");

        var appointment = await _appointmentRepository.GetAsync(id);

        if (appointment is null)
            throw new NotFoundException($"Appointment with id '{id}' was not found.");

        return _appointmentMapper.ToDto(appointment);
    }
}