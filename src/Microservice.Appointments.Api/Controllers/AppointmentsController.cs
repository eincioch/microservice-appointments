using Microservice.Appointments.Application.UseCases.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace Microservice.Appointments.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class AppointmentsController(IGetAppointmentsUseCase appointmentsUseCase) : ControllerBase
{
    private readonly IGetAppointmentsUseCase _appointmentsUseCase = appointmentsUseCase;

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Get()
    {
        var appointments = await _appointmentsUseCase.ExecuteAsync();
        return Ok(appointments);
    }
}