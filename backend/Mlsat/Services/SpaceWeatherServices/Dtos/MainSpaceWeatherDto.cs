using Mlsat.Models.Entities.SpaceWeather;

namespace Mlsat.Services.SpaceWeatherServices.Dtos;

public class MainSpaceWeatherDto
{
    public IList<Dst> Dst { get; set; } = new List<Dst>();
    public IList<Kp> Kp { get; set; } = new List<Kp>();
    public IList<Ap> Ap { get; set; } = new List<Ap>();
}