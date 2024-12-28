using Microservice.Appointments.Api.Helpers;
using Microservice.Appointments.Api.Requests;
using Microservice.Appointments.Application.Dtos.Appointments;
using Microservice.Appointments.Application.UseCases.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace Microservice.Appointments.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class AppointmentsController(IGetAppointmentsUseCase appointmentsUseCase,
                                    IGetAppointmentByIdUseCase getAppointmentByIdUseCase,
                                    ICreateAppointmentUseCase createAppointmentUseCase,
                                    IUpdateAppointmentUseCase updateAppointmentUseCase,
                                    IUpdateAppointmentStatusUseCase updateAppointmentStatusUseCase,
                                    IDeleteAppointmentUseCase deleteAppointmentUseCase) : ControllerBase
{
    private readonly IGetAppointmentsUseCase _appointmentsUseCase = appointmentsUseCase;
    private readonly IGetAppointmentByIdUseCase _getAppointmentByIdUseCase = getAppointmentByIdUseCase;
    private readonly ICreateAppointmentUseCase _createAppointmentUseCase = createAppointmentUseCase;
    private readonly IUpdateAppointmentUseCase _updateAppointmentUseCase = updateAppointmentUseCase;
    private readonly IUpdateAppointmentStatusUseCase _updateAppointmentStatusUseCase = updateAppointmentStatusUseCase;
    private readonly IDeleteAppointmentUseCase _deleteAppointmentUseCase = deleteAppointmentUseCase;

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<AppointmentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<AppointmentDto>>> GetAll()
    {
        var appointments = await _appointmentsUseCase.ExecuteAsync();
        return Ok(appointments);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(AppointmentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<AppointmentDto>> GetById(int id)
    {
        var appointment = await _getAppointmentByIdUseCase.ExecuteAsync(id);
        return Ok(appointment);
    }

    [HttpPost]
    [ProducesResponseType(typeof(AppointmentDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Create([FromBody] CreateAppointmentRequest request)
    {
        var appointment = await _createAppointmentUseCase.ExecuteAsync(
            request.Title,
            request.StartTime,
            request.EndTime,
            request.Description
        );
        return ActionResultHelper.Created(nameof(GetById), ControllerContext.ActionDescriptor.ControllerName, appointment.Id, appointment);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(AppointmentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<AppointmentDto>> Update(int id, [FromBody] UpdateAppointmentRequest request)
    {
        var result = await _updateAppointmentUseCase.ExecuteAsync(
            id,
            request.Title,
            request.StartTime,
            request.EndTime,
            request.Description,
            request.Status
        );
        return Ok(result);
    }

    [HttpPatch("{id}/status")]
    [ProducesResponseType(typeof(AppointmentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<AppointmentDto>> UpdateStatus(int id, [FromBody] UpdateAppointmentStatusRequest request)
    {
        var result = await _updateAppointmentStatusUseCase.ExecuteAsync(id, request.Status);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Delete(int id)
    {
        await _deleteAppointmentUseCase.ExecuteAsync(id);
        return NoContent();
    }
}