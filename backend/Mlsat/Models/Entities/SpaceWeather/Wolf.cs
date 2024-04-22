namespace Mlsat.Models.Entities.SpaceWeather;

public class Wolf : ISpaceWeatherPoint
{
    public DateTime Date { get; set; }
    public decimal? Value { get; set; }
}