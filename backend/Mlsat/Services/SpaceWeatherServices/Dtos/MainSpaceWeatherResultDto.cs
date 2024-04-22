using Mlsat.Models.Entities.SpaceWeather;

namespace Mlsat.Services.SpaceWeatherServices.Dtos;

public class MainSpaceWeatherResultDto
{
    public List<DataDto> data { get; set; }

    private const string Dst = "ground.indices.dst";
    private const string Kp = "ground.indices.kp";
    private const string Ap = "ground.indices.ap";

    public class DataDto
    {
        public string request { get; set; } = default!;
        public decimal?[][] response { get; set; } = default!;
    }

    public MainSpaceWeatherDto GetMainSpaceWeather()
    {
        return new MainSpaceWeatherDto
        {
            Kp = GetSpaceWeatherPoints<Kp>(d => d.request == Kp),
            Dst = GetSpaceWeatherPoints<Dst>(d => d.request == Dst),
            Ap = GetSpaceWeatherPoints<Ap>(d => d.request == Ap)
        };
    }

    private readonly DateTime _unixDateTime
        = new(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    private List<T> GetSpaceWeatherPoints<T>(Func<DataDto, bool> predicate) where T : ISpaceWeatherPoint, new()
    {
        var points = new LinkedList<T>();
        var indices = data.Where(predicate);

        foreach (var data in indices)
        {
            foreach (var value in data.response)
            {
                var milliseconds = (double)value[0]!.Value;
                var date = _unixDateTime.AddMilliseconds(milliseconds);

                var point = new T
                {
                    Date = date,
                    Value = value[1]
                };

                points.AddLast(point);
            }
        }

        return points.Where(p => p.Value != null).ToList();
    }
}