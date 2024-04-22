using MediatR;
using Microsoft.EntityFrameworkCore;
using Mlsat.Models.Entities.SpaceWeather;
using Mlsat.Services;
using Mlsat.Services.SpaceWeatherServices;

namespace Mlsat.Features.SpaceWeather.Command.SyncSpaceWeather;

public class SyncSpaceWeatherCommand : IRequest
{
    public DateTime StartDate { get; set; } = new DateTime(2016, 1, 1);
    public DateTime EndDate { get; set; } = new DateTime(2024, 4, 13);
}

public class SyncSpaceWeatherCommandHandler : IRequestHandler<SyncSpaceWeatherCommand>
{
    private readonly AppDbContext _context;
    private readonly SpaceWeatherService _spaceWeatherService;

    public SyncSpaceWeatherCommandHandler( 
        AppDbContext context,
        SpaceWeatherService spaceWeatherService)
    {
        _context = context;
        _spaceWeatherService = spaceWeatherService;
    }

    public async Task Handle(SyncSpaceWeatherCommand request, CancellationToken cancellationToken)
    {
        var spaceWeather = await _spaceWeatherService.GetSpaceWeather(request.StartDate, request.EndDate);
        await AddOnlyNew(spaceWeather.aps);
        await AddOnlyNew(spaceWeather.kps);
        await AddOnlyNew(spaceWeather.dsts);
        await AddOnlyNew(spaceWeather.wolfs);

        await _context.SaveChangesAsync(cancellationToken);
    }

    private async Task AddOnlyNew<T>(IReadOnlyCollection<T> spaceWeatherPoints) where T : class, ISpaceWeatherPoint
    {
        if (!spaceWeatherPoints.Any()) return;

        var startDate = spaceWeatherPoints.Min(x => x.Date);
        var endDate = spaceWeatherPoints.Max(x => x.Date);

        var existPoints = await GetSpaceWeatherPoints<T>(startDate, endDate);

        var newPoints = spaceWeatherPoints.ExceptBy(existPoints, x => x.Date);

        await _context.Set<T>().AddRangeAsync(newPoints);
    }

    private async Task<HashSet<DateTime>> GetSpaceWeatherPoints<T>(DateTime startDate, DateTime endDate)
        where T : class, ISpaceWeatherPoint 
    {
        return (await _context.Set<T>()
                .Where(x => startDate <= x.Date && x.Date <= endDate)
                .Select(x => x.Date)
                .ToListAsync())
            .ToHashSet();
    }
}