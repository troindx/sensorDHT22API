using System;
using System.Device.Gpio;
using Iot.Device.DHTxx;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using UnitsNet;

namespace Dht22Reader {
    public class Dht22Service
    {
        private readonly ILogger _logger;
        private readonly Dht22 _dht22;

        public Dht22Service(ILogger logger, IOptions<Dht22Settings> dht22Settings)
        {
            _logger = logger;
            int pin = dht22Settings.Value.Pin;
            var controller = new GpioController();
            _dht22 = new Dht22(pin);
        }

        public (Temperature Temperature, RelativeHumidity Humidity)? ReadSensorData()
        {
            try
            {
                Temperature temperature = default;
                RelativeHumidity humidity = default;
                bool success = _dht22.TryReadHumidity(out humidity) && _dht22.TryReadTemperature(out temperature);

                if (!success){
                    _logger.LogError($"Error reading DHT22 sensor: {temperature}, {humidity} ");
                    return null;
                }

                _logger.LogInformation($"Temperature: {temperature}°C, Humidity: {humidity}%");
                return (temperature, humidity);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error reading DHT22 sensor: {ex.Message}");
                return null;
            }
        }
    }
}
