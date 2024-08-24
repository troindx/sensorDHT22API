using Microsoft.AspNetCore.Mvc;
using SensorDataAPI.Models;
using SensorDataAPI.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

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
        public async Task<ActionResult<IEnumerable<SensorReading>>> Get()
        {
            var readings = await _service.GetAllAsync();
            return Ok(readings);
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

        [HttpPost]
        public async Task<ActionResult<SensorReading>> Post(SensorReading reading)
        {
            await _service.CreateAsync(reading);
            return CreatedAtAction(nameof(Get), new { id = reading.Id }, reading);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, SensorReading reading)
        {
            var existingReading = await _service.GetByIdAsync(id);

            if (existingReading == null)
            {
                return NotFound();
            }

            await _service.UpdateAsync(id, reading);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var reading = await _service.GetByIdAsync(id);

            if (reading == null)
            {
                return NotFound();
            }

            await _service.DeleteAsync(id);

            return NoContent();
        }
    }
}
