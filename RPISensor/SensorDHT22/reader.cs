using System;
using System.Device.Gpio;
using Iot.Device.DHTxx;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using UnitsNet;

namespace Dht22Reader {
    public class Dht22Service : IDisposable
    {
        private readonly ILogger _logger;
        private readonly Dht22 _dht22;

        private GpioController _controller;
        private bool _disposed = false;
        private int _pin;

        public Dht22Service(ILogger logger, IOptions<Dht22Settings> dht22Settings)
        {
            _logger = logger;
            _logger.LogInformation($"DHT22 Pin: {_pin}");
            _pin = dht22Settings.Value.Pin;
            _controller = new GpioController();
            try
            {
                 _controller.OpenPin(_pin, PinMode.Output);
                 _dht22 = new Dht22(_pin);
            }
            catch (System.Exception error)
            {
                logger.LogError($"Error on DHT22Service constructor: {error.Message}");
                throw;
            }
            
        }

         ~Dht22Service()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);  // Prevent finalizer from running
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Dispose managed state (managed objects)
                    if (_controller != null)
                    {
                        if (_controller.IsPinOpen(_pin))
                        {
                            _controller.ClosePin(_pin);
                            _logger.LogInformation($"DHT22Service closed pin: {_pin}");
                        }
                        _controller.Dispose();
                    }
                }
                _disposed = true;
            }
        }

        public (Temperature Temperature, RelativeHumidity Humidity)? ReadSensorData()
        {
            try
            {
                Temperature temperature = default;
                RelativeHumidity humidity = default;
                bool success = _dht22.TryReadHumidity(out humidity) && _dht22.TryReadTemperature(out temperature);

                if (!success){
                    _logger.LogError($"Could not read from DHT22 sensor: {temperature}, {humidity} ");
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
