using Microsoft.AspNetCore.Mvc;
using SensorDataAPI.Models;
using SensorDataAPI.Services;

namespace SensorDataAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SensorReadingsController : ControllerBase
    {
        private readonly ISensorReadingService _service;

        public SensorReadingsController(ISensorReadingService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            [FromQuery] int pageSize = 10,
            [FromQuery] int pageNumber = 0)
        {
            var sensorReadings = await _service.GetAllAsync(pageSize, pageNumber,startDate, endDate );

            return Ok(sensorReadings);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SensorReading>> Get(int id)
        {
            var reading = await _service.GetByIdAsync(id);

            if (reading == null)
            {
                return NotFound();
            }

            return Ok(reading);
        }
    }
}
