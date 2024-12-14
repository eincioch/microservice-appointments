using Microsoft.AspNetCore.Mvc;

namespace Microservice.Appointments.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AppointmentsController(ILogger<AppointmentsController> logger) : ControllerBase
    {
        private readonly ILogger<AppointmentsController> _logger = logger;

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult Get()
        {
            // TODO: Replace with real logic (fetch appointments from the use case).
            _logger.LogInformation("Fetching appointments...");

            var appointments = new List<object> { new { Id = decimal.Zero, Date = DateTime.UtcNow } };

            return Ok(appointments);
        }
    }
}