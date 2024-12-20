using Microservice.Appointments.Application.UseCases.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace Microservice.Appointments.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class AppointmentsController(IGetAppointmentsUseCase appointmentsUseCase, IGetAppointmentByIdUseCase _getAppointmentByIdUseCase) : ControllerBase
{
    private readonly IGetAppointmentsUseCase _appointmentsUseCase = appointmentsUseCase;
    private readonly IGetAppointmentByIdUseCase _getAppointmentByIdUseCase = _getAppointmentByIdUseCase;

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAll()
    {
        var appointments = await _appointmentsUseCase.ExecuteAsync();
        return Ok(appointments);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetById(int id)
    {
        var appointments = await _getAppointmentByIdUseCase.ExecuteAsync(id);
        return Ok(appointments);
    }
}