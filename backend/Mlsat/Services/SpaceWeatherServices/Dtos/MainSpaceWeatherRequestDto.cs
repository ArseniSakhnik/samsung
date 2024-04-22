namespace Mlsat.Services.SpaceWeatherServices.Dtos;

public class MainSpaceWeatherRequestDto
{
    public RequestDto request;

    public MainSpaceWeatherRequestDto(DateTime startDate, DateTime endDate)
    {
        request = new RequestDto(startDate, endDate);
    }

    public class RequestDto
    {
        public IEnumerable<string> select = new[]
        {
            "ground.indices.dst",
            "ground.indices.kp",
            "ground.indices.ap",
        };
        public Where where;
        public Options options = new();


        public RequestDto(DateTime startDate, DateTime endDate)
        {
            where = new Where(startDate, endDate);
        }

        public class Where
        {
            public long min_dt;
            public long max_dt;
            public string resolution = "10s";

            public Where(DateTime minDt, DateTime maxDt)
            {
                min_dt = ((DateTimeOffset)minDt).ToUnixTimeMilliseconds();
                max_dt = ((DateTimeOffset)maxDt).ToUnixTimeMilliseconds();
            }
        }

        public class Options
        {
            public string unformated = "true";
        }
    }
}