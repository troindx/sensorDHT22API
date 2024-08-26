using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using SensorDataAPI.Data;
using SensorDataAPI.Models;
using SensorDataAPI.Services;
using Xunit;

namespace SensorDataAPI.Tests
{
    public class SensorReadingServiceIntegrationTests : IDisposable
    {
        private readonly SensorReadingService _service;
        private readonly SensorDataContext _context;

        public SensorReadingServiceIntegrationTests()
        {
            var options = new DbContextOptionsBuilder<SensorDataContext>()
                //.UseMySql("Server=localhost;Database=SensorData;User=sensor_user;Password=sensor_password;", 
                //          new MySqlServerVersion(new Version(8, 0, 21))) // Set up the DbContext with a connection string to the Docker container
                .UseInMemoryDatabase("SensorDataTestDb") // or Use InMemory database for testing
                .Options;

            _context = new SensorDataContext(options);
            _context.Database.EnsureCreated(); // Ensure the database is created

            _service = new SensorReadingService(_context);
        }

        [Fact]
        public async Task CreateAsync_ShouldAddNewReading()
        {
            // Arrange
            var reading = new SensorReading
            {
                Time = DateTime.Now,
                Temperature = 22.5,
                Humidity = 45
            };

            // Act
            await _service.CreateAsync(reading);
            var result = await _service.GetAllAsync();

            // Assert
            Assert.Single(result);
            Assert.Equal(22.5, result.First().Temperature);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnCorrectReading()
        {
            // Arrange
            var reading = new SensorReading
            {
                Time = DateTime.Now,
                Temperature = 23.5,
                Humidity = 50
            };

            await _service.CreateAsync(reading);
            var createdReading = await _service.GetAllAsync();
            var id = createdReading.First().Id;

            // Act
            var result = await _service.GetByIdAsync(id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(23.5, result.Temperature);
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateExistingReading()
        {
            // Arrange
            var reading = new SensorReading
            {
                Time = DateTime.Now,
                Temperature = 22.5,
                Humidity = 45
            };

            await _service.CreateAsync(reading);
            var createdReading = (await _service.GetAllAsync()).FirstOrDefault();
            Assert.NotNull(createdReading);
            var updatedReading = new SensorReading
            {
                Time = DateTime.Now.AddHours(1),
                Temperature = 25.0,
                Humidity = 50
            };

            // Act
            await _service.UpdateAsync(createdReading.Id, updatedReading);
            var result = await _service.GetByIdAsync(createdReading.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(25.0, result.Temperature);
            Assert.Equal(50, result.Humidity);
            Assert.Equal(updatedReading.Time, result.Time);
        }

        [Fact]
        public async Task DeleteAsync_ShouldRemoveReading()
        {
            // Arrange
            var reading = new SensorReading
            {
                Time = DateTime.Now,
                Temperature = 22.5,
                Humidity = 45
            };

            await _service.CreateAsync(reading);
            var createdReading = (await _service.GetAllAsync()).FirstOrDefault();
            Assert.NotNull(createdReading);

            // Act
            await _service.DeleteAsync(createdReading.Id);
            var result = await _service.GetByIdAsync(createdReading.Id);

            // Assert
            Assert.Null(result);
        }
        [Fact]
        public async Task GetAllAsync_ShouldReturnFilteredAndPaginatedResults()
        {
            // Arrange: Add some sensor readings with different dates
            var readings = new List<SensorReading>
            {
                new SensorReading { Time = DateTime.UtcNow.AddDays(-3), Temperature = 20.5, Humidity = 40 },
                new SensorReading { Time = DateTime.UtcNow.AddDays(-2), Temperature = 22.5, Humidity = 45 },
                new SensorReading { Time = DateTime.UtcNow.AddDays(-1), Temperature = 24.0, Humidity = 50 },
                new SensorReading { Time = DateTime.UtcNow, Temperature = 25.0, Humidity = 55 }
            };

            foreach (var reading in readings)
            {
                await _service.CreateAsync(reading);
            }

            var startDate = DateTime.UtcNow.AddDays(-2);
            var endDate = DateTime.UtcNow;
            var pageSize = 2;
            var pageNumber = 0;

            // Act: Retrieve filtered and paginated sensor readings
            var result = await _service.GetAllAsync(pageSize, pageNumber, startDate, endDate );

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2); // Should only return 2 readings per page
            result.First().Temperature.Should().Be(24); // The first reading on page 1
            result.Last().Temperature.Should().Be(25); // The last reading on page 1
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnCorrectPage()
        {
            // Arrange: Add some sensor readings with different dates
            var readings = new List<SensorReading>
            {
                new SensorReading { Time = DateTime.UtcNow.AddDays(-3), Temperature = 20.5, Humidity = 40 },
                new SensorReading { Time = DateTime.UtcNow.AddDays(-2), Temperature = 22.5, Humidity = 45 },
                new SensorReading { Time = DateTime.UtcNow.AddDays(-1), Temperature = 24.0, Humidity = 50 },
                new SensorReading { Time = DateTime.UtcNow, Temperature = 25.0, Humidity = 55 }
            };

            foreach (var reading in readings)
            {
                await _service.CreateAsync(reading);
            }

            var startDate = DateTime.UtcNow.AddDays(-3);
            var endDate = DateTime.UtcNow;
            var pageSize = 2;
            var pageNumber = 0;

            // Act: Retrieve the second page of sensor readings
            var result = await _service.GetAllAsync(pageSize, pageNumber, startDate, endDate);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2); // Should only return 2 readings
            result.First().Temperature.Should().Be(22.5); // The first reading on page 2
            result.Last().Temperature.Should().Be(24.0); // The last reading on page 2
        }


        public void Dispose()
        {
            // Clean up the database after each test
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
