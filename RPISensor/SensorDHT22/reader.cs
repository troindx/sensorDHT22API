using System.Diagnostics;
using Microsoft.Extensions.Options;
using SensorDataAPI.Models;
using SensorDataAPI.Services;
using UnitsNet;

namespace Dht22Reader
{
    public class Dht22Service
    {
        private readonly ILogger<Dht22Service> _logger;
        private readonly string _executablePath;
        private readonly int _pin;
        private readonly IServiceScopeFactory _scopeFactory;

        public Dht22Service(ILogger<Dht22Service> logger, IOptions<Dht22Settings> dht22Settings, IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _executablePath = dht22Settings.Value.ExecutablePath;
            _pin = dht22Settings.Value.Pin;
            _scopeFactory = scopeFactory;

            _logger.LogInformation($"DHT22 executable path: {_executablePath}");
            _logger.LogInformation($"DHT22 GPIO pin: {_pin}");
        }

        ~Dht22Service()
        {
        }

        public async Task WriteSensorData(Temperature Temperature, RelativeHumidity Humidity)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var _dbService = scope.ServiceProvider.GetRequiredService<ISensorReadingService>();
                try
                {
                    var reading = new SensorReading
                    {
                        Time = DateTime.UtcNow,
                        Temperature = Temperature.DegreesCelsius,
                        Humidity = Humidity.Percent
                    };
                    await _dbService.CreateAsync(reading);
                }
                catch (System.Exception err)
                {
                    _logger.LogError( err.Message);
                    throw;
                }
            }
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
                            _logger.LogWarning("Could not read temperature.");
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

        public async Task ReadAndWriteSensorData(){
            var sensorData = ReadSensorData();
            if(sensorData.HasValue){
                var (Temperature,Humidity) = sensorData.Value;
                await WriteSensorData(Temperature, Humidity);
            }
        }
    }
}
