using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using WeatherApp.Controllers;
using WeatherApp.Data;
using WeatherApp.Models;

namespace WeatherApp.Tests
{
    [TestFixture]
    public class WeatherControllerTests
    {
        private WeatherController _weatherController;
        private ILogger<WeatherController> _logger;

        [SetUp]
        public void Setup()
        {
            var loggerMock = new Mock<ILogger<WeatherController>>();
            _logger = loggerMock.Object;

            var dbContextOptions = new DbContextOptionsBuilder<WeatherDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var dbContext = new WeatherDbContext(dbContextOptions);

            var httpClientFactoryMock = new Mock<IHttpClientFactory>();

            _weatherController = new WeatherController(_logger, dbContext, httpClientFactoryMock.Object);
        }

        [Test]
        public async Task GetWeather_ExistingRecord_ReturnsOkWithWeather()
        {
            var dbContextOptions = new DbContextOptionsBuilder<WeatherDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using (var dbContext = new WeatherDbContext(dbContextOptions))
            {
                var existingWeather = new Weather
                {
                    Id = 1,
                    Location = "New York",
                    Description = "Example Description",
                    UpdatedAt = new DateTime(2022, 1, 1)
                };
                dbContext.Weathers.Add(existingWeather);
                dbContext.SaveChanges();

                var loggerMock = new Mock<ILogger<WeatherController>>();
                _logger = loggerMock.Object;

                var httpClientFactoryMock = new Mock<IHttpClientFactory>();

                _weatherController = new WeatherController(_logger, dbContext, httpClientFactoryMock.Object);

                // Test code...

                // Act
                var result = await _weatherController.GetWeather(1);

                // Assert
                Assert.NotNull(result);
                Assert.IsInstanceOf<OkObjectResult>(result);

                var okResult = result as OkObjectResult;
                Assert.NotNull(okResult.Value);
                Assert.IsInstanceOf<Weather>(okResult.Value);

                var weather = okResult.Value as Weather;
                Assert.That(weather.Id, Is.EqualTo(1));
                Assert.That(weather.Location, Is.EqualTo("New York"));
                Assert.That(weather.UpdatedAt, Is.EqualTo(new DateTime(2022, 1, 1)));
            }
        }

        [Test]
        public async Task GetWeather_NonExistingRecord_ReturnsNotFound()
        {
            // Act
            var result = await _weatherController.GetWeather(1);

            // Assert
            Assert.NotNull(result);
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public async Task CreateWeather_ValidWeather_ReturnsCreatedAtActionWithWeather()
        {
            // Arrange
            var newWeather = new Weather
            {
                Location = "London",
                Description = "Sunny", // Set the Description property here
                UpdatedAt = DateTime.Now
            };

            // Act
            var result = await _weatherController.CreateWeather(newWeather);

            // Assert
            Assert.NotNull(result);
            Assert.IsInstanceOf<CreatedAtActionResult>(result);

            var createdAtActionResult = result as CreatedAtActionResult;
            Assert.NotNull(createdAtActionResult.Value);
            Assert.IsInstanceOf<Weather>(createdAtActionResult.Value);

            var weather = createdAtActionResult.Value as Weather;
            Assert.AreEqual("London", weather.Location);
        }

        [Test]
        public async Task UpdateWeather_ExistingRecord_ReturnsOkWithUpdatedWeather()
        {
            var dbContextOptions = new DbContextOptionsBuilder<WeatherDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using (var dbContext = new WeatherDbContext(dbContextOptions))
            {
                var existingWeather = new Weather
                {
                    Id = 1,
                    Location = "Paris",
                    Description = "Example Description",
                    UpdatedAt = new DateTime(2022, 1, 1)
                };
                dbContext.Weathers.Add(existingWeather);
                dbContext.SaveChanges();

                var loggerMock = new Mock<ILogger<WeatherController>>();
                _logger = loggerMock.Object;

                var httpClientFactoryMock = new Mock<IHttpClientFactory>();

                _weatherController = new WeatherController(_logger, dbContext, httpClientFactoryMock.Object);

                // Test code...

                var updatedWeather = new Weather
                {
                    Location = "Paris",
                    Description = "Updated Description", // Set the Description property here
                    UpdatedAt = DateTime.Now
                };

                // Act
                var result = await _weatherController.UpdateWeather(1, updatedWeather);

                // Assert
                Assert.NotNull(result);
                Assert.IsInstanceOf<OkObjectResult>(result);

                var okResult = result as OkObjectResult;
                Assert.NotNull(okResult.Value);
                Assert.IsInstanceOf<Weather>(okResult.Value);

                var weather = okResult.Value as Weather;
                Assert.AreEqual(1, weather.Id);
                Assert.AreEqual("Paris", weather.Location);
                Assert.AreNotEqual(new DateTime(2022, 1, 1), weather.UpdatedAt);
            }
        }

        [Test]
        public async Task UpdateWeather_NonExistingRecord_ReturnsNotFound()
        {
            // Arrange
            var updatedWeather = new Weather
            {
                Location = "Berlin",
                UpdatedAt = DateTime.Now
            };

            // Act
            var result = await _weatherController.UpdateWeather(1, updatedWeather);

            // Assert
            Assert.NotNull(result);
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public async Task DeleteWeather_ExistingRecord_ReturnsNoContent()
        {
            var dbContextOptions = new DbContextOptionsBuilder<WeatherDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using (var dbContext = new WeatherDbContext(dbContextOptions))
            {
                var existingWeather = new Weather
                {
                    Id = 1,
                    Location = "Berlin",
                    Description = "Example Description",
                    UpdatedAt = new DateTime(2022, 1, 1)
                };
                dbContext.Weathers.Add(existingWeather);
                dbContext.SaveChanges();

                var loggerMock = new Mock<ILogger<WeatherController>>();
                _logger = loggerMock.Object;

                var httpClientFactoryMock = new Mock<IHttpClientFactory>();

                _weatherController = new WeatherController(_logger, dbContext, httpClientFactoryMock.Object);

                // Act
                var result = await _weatherController.DeleteWeather(1);

                // Assert
                Assert.NotNull(result);
                Assert.IsInstanceOf<NoContentResult>(result);
            }
        }

        [Test]
        public async Task DeleteWeather_NonExistingRecord_ReturnsNotFound()
        {
            // Act
            var result = await _weatherController.DeleteWeather(1);

            // Assert
            Assert.NotNull(result);
            Assert.IsInstanceOf<NotFoundResult>(result);
        }
    }
}