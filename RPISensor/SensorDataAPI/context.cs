using Microsoft.EntityFrameworkCore;
using SensorDataAPI.Models;

namespace SensorDataAPI.Data
{
    public class SensorDataContext : DbContext
    {
        public SensorDataContext(DbContextOptions<SensorDataContext> options) : base(options)
        {
        }

        public DbSet<SensorReading> SensorReadings { get; set; }
    }
}
