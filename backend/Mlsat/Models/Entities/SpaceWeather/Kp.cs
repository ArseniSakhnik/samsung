namespace Mlsat.Models.Entities.SpaceWeather;

public class Kp : ISpaceWeatherPoint
{
    public DateTime Date { get; set; }
    public decimal? Value { get; set; }
}