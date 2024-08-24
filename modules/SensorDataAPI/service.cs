using Microsoft.EntityFrameworkCore;
using SensorDataAPI.Data;
using SensorDataAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SensorDataAPI.Services
{
    public class SensorReadingService : ISensorReadingService
    {
        private readonly SensorDataContext _context;

        public SensorReadingService(SensorDataContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<SensorReading>> GetAllAsync()
        {
            return await _context.SensorReadings.ToListAsync();
        }

        public async Task<SensorReading?> GetByIdAsync(int id)
        {
            return await _context.SensorReadings.FindAsync(id);
        }

        public async Task CreateAsync(SensorReading reading)
        {
            _context.SensorReadings.Add(reading);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(int id, SensorReading reading)
        {
            var existingReading = await _context.SensorReadings.FindAsync(id);
            if (existingReading == null) return;

            existingReading.Time = reading.Time;
            existingReading.Temperature = reading.Temperature;
            existingReading.Humidity = reading.Humidity;

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var reading = await _context.SensorReadings.FindAsync(id);
            if (reading != null)
            {
                _context.SensorReadings.Remove(reading);
                await _context.SaveChangesAsync();
            }
        }
    }
}
