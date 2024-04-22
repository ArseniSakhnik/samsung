namespace Mlsat.Models.Entities.SpaceWeather;

public interface ISpaceWeatherPoint
{
    public DateTime Date { get; set; }
    public decimal? Value { get; set; }
}