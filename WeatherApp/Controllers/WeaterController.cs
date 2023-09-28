using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using WeatherApp.Data;
using WeatherApp.Models;
using System.Net.Http;
using System.Diagnostics;

namespace WeatherApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WeatherController : ControllerBase
    {
        private readonly ILogger<WeatherController> _logger;
        private readonly WeatherDbContext _dbContext;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly HttpClient _httpClient;
        private Timer _refreshTimer;

        public WeatherController(ILogger<WeatherController> logger, WeatherDbContext dbContext, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _dbContext = dbContext;
            _httpClientFactory = httpClientFactory;
            _httpClient = _httpClientFactory.CreateClient("WeatherAPI");

            // Schedule the initial refresh and set the interval
            int refreshInterval = 60 * 60 * 1000; // 1 hour
            _refreshTimer = new Timer(RefreshWeatherData, null, TimeSpan.Zero, TimeSpan.FromMilliseconds(refreshInterval));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetWeather(int id)
        {
            var weather = await _dbContext.Weathers.FindAsync(id);
            if (weather == null)
            {
                return NotFound();
            }

            return Ok(weather);
        }

        [HttpPost]
        public async Task<IActionResult> CreateWeather([FromBody] Weather weather)
        {
            _dbContext.Weathers.Add(weather);
            await _dbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetWeather), new { id = weather.Id }, weather);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateWeather(int id, [FromBody] Weather updatedWeather)
        {
            var weather = await _dbContext.Weathers.FindAsync(id);
            if (weather == null)
            {
                return NotFound();
            }

            weather.Location = updatedWeather.Location;
            weather.UpdatedAt = DateTime.Now;
            // Update other properties as required

            await _dbContext.SaveChangesAsync();

            return Ok(weather);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWeather(int id)
        {
            var weather = await _dbContext.Weathers.FindAsync(id);
            if (weather == null)
            {
                return NotFound();
            }

            _dbContext.Weathers.Remove(weather);
            await _dbContext.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshWeatherData()
        {
            try
            {
                await RefreshWeatherDataJob();
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to refresh weather data");
                return StatusCode(500, "Failed to refresh weather data");
            }
        }
        
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task RefreshWeatherDataJob()
        {
            try
            {
                var locations = await _dbContext.Weathers.Select(w => w.Location).Distinct().ToListAsync();

                foreach (var location in locations)
                {
                    WeatherData weatherData = await RetrieveWeatherDataFromAPI(location);
                    if (weatherData != null)
                    {
                        Weather existingWeather = await _dbContext.Weathers.FirstOrDefaultAsync(w => w.Location == location);
                        if (existingWeather != null)
                        {
                            // Update existing weather record
                            existingWeather.UpdatedAt = DateTime.Now;
                            // Update other properties as required
                        }
                        else
                        {
                            // Create a new weather record
                            Weather newWeather = new Weather
                            {
                                Location = weatherData.Location.Name,
                                UpdatedAt = DateTime.Now,
                                // Set other properties as required
                            };
                            _dbContext.Weathers.Add(newWeather);
                        }
                    }
                }

                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to refresh weather data");
            }
        }
        
        [HttpPost("search")]
        public async Task<IActionResult> SearchWeatherData([FromBody] SearchRequest searchRequest)
        {
            try
            {
                string location = searchRequest.City ?? searchRequest.ZipCode;
                if (string.IsNullOrEmpty(location))
                {
                    return BadRequest("City or zip code is required");
                }

                WeatherData weatherData = await RetrieveWeatherDataFromAPI(location);
                if (weatherData != null)
                {
                    Weather existingWeather = await _dbContext.Weathers.FirstOrDefaultAsync(w => w.Location == location);
                    if (existingWeather != null)
                    {
                        // Update existing weather record
                        existingWeather.UpdatedAt = DateTime.Now;
                        // Update other properties as required
                    }
                    else
                    {
                        // Create a new weather record
                        Weather newWeather = new Weather
                        {
                            Location = weatherData.Location.Name,
                            UpdatedAt = DateTime.Now,
                            // Set other properties as required
                        };
                        _dbContext.Weathers.Add(newWeather);
                    }

                    await _dbContext.SaveChangesAsync();

                    return Ok("Weather data saved successfully");
                }

                return NotFound("No weather data found");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to search weather data");
                return StatusCode(500, "Failed to search weather data");
            }
        }
        private async void RefreshWeatherData(object state)
        {
            try
            {
                // Implement the logic to refresh weather data
                var locations = await _dbContext.Weathers.Select(w => w.Location).Distinct().ToListAsync();

                foreach (var location in locations)
                {
                    WeatherData weatherData = await RetrieveWeatherDataFromAPI(location);
                    if (weatherData != null)
                    {
                        Weather existingWeather = await _dbContext.Weathers.FirstOrDefaultAsync(w => w.Location == location);
                        if (existingWeather != null)
                        {
                            // Update existing weather record
                            existingWeather.UpdatedAt = DateTime.Now;
                            // Update other properties as required
                        }
                        else
                        {
                            // Create a new weather record
                            Weather newWeather = new Weather
                            {
                                Location = weatherData.Location.Name,
                                UpdatedAt = DateTime.Now,
                                // Set other properties as required
                            };
                            _dbContext.Weathers.Add(newWeather);
                        }
                    }
                }

                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to refresh weather data");
            }
        }

        private async Task<WeatherData> RetrieveWeatherDataFromAPI(string location)
        {
            var response = await _httpClient.GetAsync($"v1/current.json?key=YOUR_API_KEY&q={location}&aqi=no");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var weatherData = JsonSerializer.Deserialize<WeatherData>(content, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });
                return weatherData;
            }

            return null;
        }
    }
}