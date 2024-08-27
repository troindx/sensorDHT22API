using System.Net.Http.Json;
using Xunit;
using FluentAssertions;
using SensorDataAPI.Models;

namespace SensorDataAPI.Tests
{
    public class SensorReadingControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public SensorReadingControllerTests(CustomWebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task CreateSensorReading_ShouldReturnCreatedReading()
        {
            // Arrange
            var newReading = new SensorReading
            {
                Time = DateTime.UtcNow,
                Temperature = 23.5,
                Humidity = 60
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/sensorreadings", newReading);

            // Assert
            response.EnsureSuccessStatusCode();
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);

            var createdReading = await response.Content.ReadFromJsonAsync<SensorReading>();
            Assert.NotNull(createdReading);
            createdReading.Id.Should().BeGreaterThan(0);
            createdReading.Time.Should().BeCloseTo(newReading.Time, TimeSpan.FromSeconds(1));
            createdReading.Temperature.Should().Be(newReading.Temperature);
            createdReading.Humidity.Should().Be(newReading.Humidity);
        }

        [Fact]
        public async Task GetAllSensorReadings_ShouldReturnListOfReadings()
        {
            // Arrange
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

            //Assert insertions were done correctly
            var result1 = await _client.PostAsJsonAsync("/api/sensorreadings", reading1);
            var result2 = await _client.PostAsJsonAsync("/api/sensorreadings", reading2);
            Assert.NotNull(result1);
            Assert.NotNull(result2);
            result1.EnsureSuccessStatusCode();
            result1.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);
            result2.EnsureSuccessStatusCode();
            result2.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);

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
        public async Task UpdateSensorReading_ShouldModifyExistingReading()
        {
            // Arrange
            var originalReading = new SensorReading
            {
                Time = DateTime.UtcNow,
                Temperature = 21.0,
                Humidity = 40
            };

            var postResponse = await _client.PostAsJsonAsync("/api/sensorreadings", originalReading);
            var createdReading = await postResponse.Content.ReadFromJsonAsync<SensorReading>();
            Assert.NotNull(createdReading);
            var updatedReading = new SensorReading
            {
                Time = createdReading.Time.AddHours(1),
                Temperature = 23.0,
                Humidity = 45
            };

            // Act
            var putResponse = await _client.PutAsJsonAsync($"/api/sensorreadings/{createdReading.Id}", updatedReading);

            // Assert
            putResponse.EnsureSuccessStatusCode();
            putResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.NoContent);

            var getResponse = await _client.GetAsync($"/api/sensorreadings/{createdReading.Id}");
            getResponse.EnsureSuccessStatusCode();
            var fetchedReading = await getResponse.Content.ReadFromJsonAsync<SensorReading>();

            Assert.NotNull(fetchedReading);
            fetchedReading.Id.Should().Be(createdReading.Id);
            fetchedReading.Time.Should().BeCloseTo(updatedReading.Time, TimeSpan.FromSeconds(1));
            fetchedReading.Temperature.Should().Be(updatedReading.Temperature);
            fetchedReading.Humidity.Should().Be(updatedReading.Humidity);
        }

        [Fact]
        public async Task DeleteSensorReading_ShouldRemoveReading()
        {
            // Arrange
            var reading = new SensorReading
            {
                Time = DateTime.UtcNow,
                Temperature = 20.0,
                Humidity = 35
            };

            var postResponse = await _client.PostAsJsonAsync("/api/sensorreadings", reading);
            var createdReading = await postResponse.Content.ReadFromJsonAsync<SensorReading>();
            Assert.NotNull(createdReading);

            // Act
            var deleteResponse = await _client.DeleteAsync($"/api/sensorreadings/{createdReading.Id}");

            // Assert
            deleteResponse.EnsureSuccessStatusCode();
            deleteResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.NoContent);

            var getResponse = await _client.GetAsync($"/api/sensorreadings/{createdReading.Id}");
            getResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
        }

    [Fact]
        public async Task GetAllEndpoint_ShouldReturnFilteredAndPaginatedResults()
        {

            // Arrange: Add some sensor readings with different dates
            var readings = new List<SensorReading>
            {
                new SensorReading { Time = DateTime.UtcNow.AddDays(-5), Temperature = 20.5, Humidity = 40 },
                new SensorReading { Time = DateTime.UtcNow.AddDays(-4), Temperature = 22.5, Humidity = 45 },
                new SensorReading { Time = DateTime.UtcNow.AddDays(-3), Temperature = 24.0, Humidity = 50 },
                new SensorReading { Time = DateTime.UtcNow, Temperature = 25.0, Humidity = 55 }
            };

            foreach (var reading in readings)
            {
                await _client.PostAsJsonAsync("/api/sensorreadings", reading);
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
            // Arrange: Add some sensor readings with different dates
            var readings = new List<SensorReading>
            {
                new SensorReading { Time = DateTime.UtcNow.AddDays(-8), Temperature = 30.5, Humidity = 40 },
                new SensorReading { Time = DateTime.UtcNow.AddDays(-7), Temperature = 32.5, Humidity = 45 },
                new SensorReading { Time = DateTime.UtcNow.AddDays(-6), Temperature = 34.0, Humidity = 50 },
                new SensorReading { Time = DateTime.UtcNow, Temperature = 25.0, Humidity = 55 }
            };

            foreach (var reading in readings)
            {
                await _client.PostAsJsonAsync("/api/sensorreadings", reading);
            }

            var startDate = DateTime.UtcNow.AddDays(-9).ToString("yyyy-MM-ddTHH:mm:ssZ");
            var endDate = DateTime.UtcNow.AddDays(2).ToString("yyyy-MM-ddTHH:mm:ssZ");
            var pageSize = 2;
            var pageNumber = 1;

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
