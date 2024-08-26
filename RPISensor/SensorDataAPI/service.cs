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

        public async Task<IEnumerable<SensorReading>> GetAllAsync(int pageSize = 10, int pageNumber = 0, DateTime? startDate = null, DateTime? endDate = null )
        {
            var query = _context.SensorReadings.AsQueryable();

            if (startDate.HasValue)
            {
                query = query.Where(sr => sr.Time >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                query = query.Where(sr => sr.Time <= endDate.Value);
            }

            return await query
                .OrderBy(sr => sr.Id) // Ordering by date
                .Skip(pageNumber * pageSize) // Skipping records for pagination
                .Take(pageSize) // Taking the specified number of records
                .ToListAsync();
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
