namespace Mlsat.Models.Entities.SpaceWeather;

public class Ap : ISpaceWeatherPoint
{
    public DateTime Date { get; set; }
    public decimal? Value { get; set; }
}