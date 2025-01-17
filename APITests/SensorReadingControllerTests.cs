using System.Net.Http.Json;
using Xunit;
using FluentAssertions;
using SensorDataAPI.Models;
using Microsoft.Extensions.DependencyInjection;
using SensorDataAPI.Services;

namespace SensorDataAPI.Tests
{
    public class SensorReadingControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly IServiceProvider _serviceProvider;


        public SensorReadingControllerTests(CustomWebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
            _serviceProvider = factory.ServiceProvider;
        }


        [Fact]
        public async Task GetAllSensorReadings_ShouldReturnListOfReadings()
        {
            // Arrange
            var service = _serviceProvider.GetRequiredService<ISensorReadingService>();

            var reading1 = new SensorReading
            {
                Time = DateTime.UtcNow,
                Temperature = 22.0,
                Humidity = 50
            };

            var reading2 = new SensorReading
            {
                Time = DateTime.UtcNow.AddMinutes(-30),
                Temperature = 24.0,
                Humidity = 55
            };

            // Inject initial data directly via the service
            await service.CreateAsync(reading1);
            await service.CreateAsync(reading2);

            // Act
            var response = await _client.GetAsync("/api/sensorreadings");

            // Assert
            response.EnsureSuccessStatusCode();
            var readings = await response.Content.ReadFromJsonAsync<List<SensorReading>>();
            Assert.NotNull(readings);
            readings.Should().NotBeNull();
            readings.Should().HaveCountGreaterOrEqualTo(2);
            readings.Should().ContainEquivalentOf(reading1, options => options.Excluding(r => r.Id));
            readings.Should().ContainEquivalentOf(reading2, options => options.Excluding(r => r.Id));
        }

        [Fact]
        public async Task GetAllEndpoint_ShouldReturnFilteredAndPaginatedResults()
        {
            // Arrange
            var service = _serviceProvider.GetRequiredService<ISensorReadingService>();

            // Add some sensor readings with different dates
            var readings = new List<SensorReading>
            {
                new SensorReading { Time = DateTime.UtcNow.AddDays(-5), Temperature = 20.5, Humidity = 40 },
                new SensorReading { Time = DateTime.UtcNow.AddDays(-4), Temperature = 22.5, Humidity = 45 },
                new SensorReading { Time = DateTime.UtcNow.AddDays(-3), Temperature = 24.0, Humidity = 50 },
                new SensorReading { Time = DateTime.UtcNow, Temperature = 25.0, Humidity = 55 }
            };

            foreach (var reading in readings)
            {
                await service.CreateAsync(reading);
            }

            var startDate = DateTime.UtcNow.AddDays(-5).ToString("yyyy-MM-ddTHH:mm:ssZ");
            var endDate = DateTime.UtcNow.AddDays(-3).ToString("yyyy-MM-ddTHH:mm:ssZ");
            var pageSize = 2;
            var pageNumber = 0;

            // Act: Retrieve filtered and paginated sensor readings
            var response = await _client.GetAsync($"/api/sensorreadings?startDate={startDate}&endDate={endDate}&pageSize={pageSize}&pageNumber={pageNumber}");
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<List<SensorReading>>();

            // Assert
            Assert.NotNull(result);
            result.Should().NotBeNull();
            result.Should().HaveCount(2); // Should only return 2 readings
            result.First().Temperature.Should().Be(22.5); // The first reading on page 1
            result.Last().Temperature.Should().Be(20.5); // The last reading on page 1
        }

        [Fact]
        public async Task GetAllEndpoint_ShouldReturnCorrectPage()
        {
            // Arrange
            var service = _serviceProvider.GetRequiredService<ISensorReadingService>();

            // Add some sensor readings with different dates
            var readings = new List<SensorReading>
            {
                new SensorReading { Time = DateTime.UtcNow.AddDays(-8), Temperature = 30.5, Humidity = 40 },
                new SensorReading { Time = DateTime.UtcNow.AddDays(-7), Temperature = 32.5, Humidity = 45 },
                new SensorReading { Time = DateTime.UtcNow.AddDays(-6), Temperature = 34.0, Humidity = 50 },
                new SensorReading { Time = DateTime.UtcNow, Temperature = 25.0, Humidity = 55 }
            };

            foreach (var reading in readings)
            {
                await service.CreateAsync(reading);
            }

            var startDate = DateTime.UtcNow.AddDays(-9).ToString("yyyy-MM-ddTHH:mm:ssZ");
            var endDate = DateTime.UtcNow.AddDays(2).ToString("yyyy-MM-ddTHH:mm:ssZ");
            var pageSize = 2;
            var pageNumber = 1;

            // Act: Retrieve the second page of filtered sensor readings
            var response = await _client.GetAsync($"/api/sensorreadings?startDate={startDate}&endDate={endDate}&pageSize={pageSize}&pageNumber={pageNumber}");
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<List<SensorReading>>();

            // Assert
            Assert.NotNull(result);
            result.Should().NotBeNull();
            result.Should().HaveCount(2); // Should only return 2 readings
            result.First().Temperature.Should().Be(32.5); // The first reading on page 2
            result.Last().Temperature.Should().Be(30.5); // The last reading on page 2
        }


    }
}
