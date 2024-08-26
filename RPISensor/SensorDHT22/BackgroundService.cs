using Dht22Reader;
using Microsoft.Extensions.Options;


public class SensorDataBackgroundService : BackgroundService
{
    private readonly ILogger<SensorDataBackgroundService> _logger;
    private readonly Dht22Service _dht22Service;
    private readonly IOptions<Dht22Settings> _settings;

    public SensorDataBackgroundService(ILogger<SensorDataBackgroundService> logger, 
                                       Dht22Service dht22Service, 
                                       IOptions<Dht22Settings> settings)
    {
        _logger = logger;
        _dht22Service = dht22Service;
        _settings = settings;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Sensor data background service is starting.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await _dht22Service.ReadAndWriteSensorData();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while reading and writing sensor data.");
            }
            await Task.Delay(TimeSpan.FromMinutes(_settings.Value.IntervalInMinutes), stoppingToken);
        }

        _logger.LogInformation("Sensor data background service is stopping.");
    }
}
