using System;
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using UnitsNet;

namespace Dht22Reader
{
    public class Dht22Service
    {
        private readonly ILogger _logger;
        private readonly string _executablePath;
        private readonly int _pin;

        public Dht22Service(ILogger logger, IOptions<Dht22Settings> dht22Settings)
        {
            _logger = logger;
            _executablePath = dht22Settings.Value.ExecutablePath;
            _pin = dht22Settings.Value.Pin;

            _logger.LogInformation($"DHT22 executable path: {_executablePath}");
            _logger.LogInformation($"DHT22 GPIO pin: {_pin}");
        }

        ~Dht22Service()
        {
        }

        public (Temperature Temperature, RelativeHumidity Humidity)? ReadSensorData()
        {
            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = $"{_executablePath}",
                    Arguments = $"{_pin}",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using (var process = Process.Start(psi))
                {
                    if (process == null) return null;
                    var output = process.StandardOutput.ReadToEnd();
                    process.WaitForExit();

                    if (process.ExitCode == 0)
                    {
                        var data = output.Trim().Split(';');
                        if (data.Length == 2 &&
                            double.TryParse(data[0], out var tempValue) &&
                            double.TryParse(data[1], out var humidityValue))
                        {
                            var temperature = Temperature.FromDegreesCelsius(tempValue);
                            var humidity = RelativeHumidity.FromPercent(humidityValue);

                            _logger.LogInformation($"Temperature: {temperature}°C, Humidity: {humidity}%");
                            return (temperature, humidity);
                        }
                        else
                        {
                            _logger.LogError("Unexpected output format");
                            return null;
                        }
                    }
                    else
                    {
                        _logger.LogError($"DHT22 reader executable failed with exit code {process.ExitCode}");
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error reading DHT22 sensor via executable: {ex.Message}");
                return null;
            }
        }
    }
}
