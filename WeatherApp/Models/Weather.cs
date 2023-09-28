using System.ComponentModel.DataAnnotations;

namespace WeatherApp.Models;

public class Weather
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Location { get; set; }

    [Required]
    public DateTime UpdatedAt { get; set; }

    [Required]
    public double Temperature { get; set; }

    public string Description { get; set; }

    public int Humidity { get; set; }

}