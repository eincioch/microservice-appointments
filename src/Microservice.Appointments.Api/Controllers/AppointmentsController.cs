using Microservice.Appointments.Api.Helpers;
using Microservice.Appointments.Application.UseCases.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace Microservice.Appointments.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class AppointmentsController(IGetAppointmentsUseCase appointmentsUseCase,
                                    IGetAppointmentByIdUseCase getAppointmentByIdUseCase,
                                    ICreateAppointmentUseCase createAppointmentUseCase) : ControllerBase
{
    private readonly IGetAppointmentsUseCase _appointmentsUseCase = appointmentsUseCase;
    private readonly IGetAppointmentByIdUseCase _getAppointmentByIdUseCase = getAppointmentByIdUseCase;
    private readonly ICreateAppointmentUseCase _createAppointmentUseCase = createAppointmentUseCase;

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

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Create(string title, DateTime startTime, DateTime endTime, string description)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var appointment = await _createAppointmentUseCase.ExecuteAsync(title, startTime, endTime, description);
        return ActionResultHelper.Created(nameof(GetById), ControllerContext.ActionDescriptor.ControllerName, appointment.Id, appointment);
    }
}