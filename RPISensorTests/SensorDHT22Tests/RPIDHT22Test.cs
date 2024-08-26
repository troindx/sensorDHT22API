using Xunit;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;
using Dht22Reader;
using UnitsNet;
using RPILogger;
using Xunit.Abstractions;
public class Dht22ServiceTests
{
    private readonly Dht22Service _dht22Service;
    private readonly IConfigurationRoot _configuration;

    public Dht22ServiceTests(ITestOutputHelper output)
    {
        _configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.Development.json")
            .Build();

        var environmentSettings = _configuration.GetSection("Dht22Settings").Get<Dht22Settings>();
        var loggerFactory = TestLoggerFactory.Create(output);
        var logger = loggerFactory.CreateLogger("RPITests");
        var dht22Settings = Options.Create(new Dht22Settings { 
            Pin = environmentSettings.Pin, 
            ExecutablePath = environmentSettings.ExecutablePath
        });
        _dht22Service = new Dht22Service(logger, dht22Settings);
    }

    [Fact]
    public void Test_ReadSensorData()
    {
        var result = _dht22Service.ReadSensorData();

        Assert.NotNull(result);

        var (Temperature, Humidity) = result.Value;

        double tempCelsius = Temperature.DegreesCelsius;
        double humidityPercent = Humidity.Percent;

        // Check temperature range -40 to 80°C
        Assert.InRange(tempCelsius, -40.0, 80.0);

        // Check humidity range 0% to 99.9%
        Assert.InRange(humidityPercent, 0.0, 99.9);
    }
}
