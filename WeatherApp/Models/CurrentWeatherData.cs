namespace WeatherApp.Models;

public class CurrentWeatherData
{
    public DateTime LastUpdated { get; set; }
    public double TemperatureCelsius { get; set; }
    public string Condition { get; set; }
    // Add more properties as per the API response
}