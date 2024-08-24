using System;

namespace SensorDataAPI.Models
{
    public class SensorReading
    {
        public int Id { get; set; }
        public DateTime Time { get; set; }
        public double Temperature { get; set; }
        public double Humidity { get; set; }
    }
}
