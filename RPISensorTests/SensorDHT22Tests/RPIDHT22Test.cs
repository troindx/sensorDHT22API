using Xunit;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;
using Dht22Reader;
using UnitsNet;
using RPILogger;
using Xunit.Abstractions;
using Microsoft.EntityFrameworkCore;
using SensorDataAPI.Data;
using SensorDataAPI.Services;
using FluentAssertions;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Moq;

public class Dht22ServiceTests
{
    private readonly Dht22Service _dht22Service;
    private readonly IConfigurationRoot _configuration;
    private readonly SensorDataContext _context;

    public Dht22ServiceTests(ITestOutputHelper output)
    {
        _configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.Development.json")
            .Build();

        var environmentSettings = _configuration.GetSection("Dht22Settings").Get<Dht22Settings>();
        var loggerFactory = TestLoggerFactory.Create(output);
        var logger = loggerFactory.CreateLogger<Dht22Service>();
        var dht22Settings = Options.Create(new Dht22Settings { 
            Pin = environmentSettings.Pin, 
            ExecutablePath = environmentSettings.ExecutablePath,
            IntervalInMinutes = environmentSettings.IntervalInMinutes
        });
        var options = new DbContextOptionsBuilder<SensorDataContext>()
            .UseInMemoryDatabase("SensorDataTestDb")
            .Options;

        _context = new SensorDataContext(options);
        _context.Database.EnsureCreated(); // Ensure the in-memory database is created

        var dbService = new SensorReadingService(_context);

        // Create a mock or real IServiceScopeFactory
        var serviceProvider = new ServiceCollection()
            .AddSingleton(dbService)
            .BuildServiceProvider();

        var serviceScopeFactoryMock = new Mock<IServiceScopeFactory>();
        var serviceScopeMock = new Mock<IServiceScope>();

        serviceScopeMock.Setup(x => x.ServiceProvider).Returns(serviceProvider);
        serviceScopeFactoryMock.Setup(x => x.CreateScope()).Returns(serviceScopeMock.Object);
        _dht22Service = new Dht22Service(logger, dht22Settings, serviceScopeFactoryMock.Object);
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

    [Fact]
    public async Task ReadAndWriteSensorData_ShouldInsertSensorReading()
    {
        await _dht22Service.ReadAndWriteSensorData();

        // Assert
        var insertedReading = await _context.SensorReadings.FirstOrDefaultAsync();
        insertedReading.Should().NotBeNull();
        // Check temperature range -40 to 80°C
        Assert.InRange(insertedReading.Temperature, -40.0, 80.0);

        // Check humidity range 0% to 99.9%
        Assert.InRange(insertedReading.Humidity, 0.0, 99.9);
    }
}
