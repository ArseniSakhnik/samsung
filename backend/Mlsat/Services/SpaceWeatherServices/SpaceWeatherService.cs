using System.Globalization;
using Mlsat.Models.Entities.SpaceWeather;
using Mlsat.Services.SpaceWeatherServices.Dtos;
using Newtonsoft.Json;

namespace Mlsat.Services.SpaceWeatherServices;

public class SpaceWeatherService
{
    private readonly HttpClient _httpClient;
    private const string Uri = "https://swx.sinp.msu.ru/db_iface/api/v1/query";
    private const string Url = "https://swx.sinp.msu.ru/gk0049/select_bd_ps.php?datetime={0}";
    private static readonly object Plug = new();

    public SpaceWeatherService()
    {
        _httpClient = new HttpClient();
    }

    public async Task<(
        IReadOnlyCollection<Ap> aps,
        IReadOnlyCollection<Kp> kps,
        IReadOnlyCollection<Dst> dsts,
        IReadOnlyCollection<Wolf> wolfs)> GetSpaceWeather(DateTime startDate, DateTime endDate)
    {
        var aps = new List<Ap>();
        var kps = new List<Kp>();
        var dsts = new List<Dst>();
        var wolfs = new List<Wolf>();

        while (startDate < endDate)
        {
            var rangeStart = startDate;
            var rangeEnd = startDate.AddDays(10);

            var mainSpaceWeather = await GetMainSpaceWeather(rangeStart, rangeEnd);
            var wolf = await GetWolf(rangeStart, rangeEnd);

            aps.AddRange(mainSpaceWeather.Ap);
            kps.AddRange(mainSpaceWeather.Kp);
            dsts.AddRange(mainSpaceWeather.Dst);
            wolfs.AddRange(wolf);

            startDate = rangeEnd;
        }

        return (aps, kps, dsts, wolfs);
    }

    private async Task<MainSpaceWeatherDto> GetMainSpaceWeather(DateTime startDate, DateTime endDate)
    {
        var requestDto = new MainSpaceWeatherRequestDto(startDate, endDate);
        using var request = new HttpRequestMessage(HttpMethod.Post, Uri);
        var stringContent = JsonConvert.SerializeObject(requestDto);
        var content = new StringContent(stringContent);
        request.Content = content;
        using var response = await _httpClient.SendAsync(request);
        var responseText = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<MainSpaceWeatherResultDto>(responseText);

        var x = result!.GetMainSpaceWeather();

        return x;
    }

    public async Task<IEnumerable<Wolf>> GetWolf(DateTime startDate, DateTime endDate)
    {
        var spaceWeatherPoints = new LinkedList<Wolf>();

        var dates = new List<DateTime>();

        for (var dt = startDate; dt <= endDate; dt = dt.AddDays(1))
        {
            dates.Add(dt);
        }

        var tasks = new List<Task>();

        foreach (var date in dates)
        {
            var taskDate = date;
            tasks.Add(Task.Run(async () => await AddPoint(taskDate, spaceWeatherPoints)));
        }

        await Task.WhenAll(tasks);

        return spaceWeatherPoints;
    }

    private async Task AddPoint(DateTime initDate, LinkedList<Wolf> points)
    {
        try
        {
            var date = initDate.ToString("yyyy/MM/dd hh:mm:ss", CultureInfo.InvariantCulture);
            var dateQuery = date.Replace(" ", "%20");
            var query = string.Format(Url, dateQuery);
            using var request = new HttpRequestMessage(HttpMethod.Get, query);
            using var response = await _httpClient.SendAsync(request);
            var responseText = await response.Content.ReadAsStringAsync();
            var wolfResultDto = JsonConvert.DeserializeObject<WolfResultDto>(responseText);

            for (var i = 0; i < wolfResultDto!.datetime.Length; i++)
            {
                var value = wolfResultDto.wolfnumber[i];

                if (value == null) continue;

                lock (Plug)
                {
                    points.AddLast(new Wolf
                    {
                        Date = wolfResultDto!.datetime[i],
                        Value = value
                    });
                }
            }
        }
        catch
        {
        }
    }
}