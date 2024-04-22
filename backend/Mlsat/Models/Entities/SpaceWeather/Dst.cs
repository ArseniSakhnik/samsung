namespace Mlsat.Models.Entities.SpaceWeather;

public class Dst : ISpaceWeatherPoint
{
    public DateTime Date { get; set; }
    public decimal? Value { get; set; }
}