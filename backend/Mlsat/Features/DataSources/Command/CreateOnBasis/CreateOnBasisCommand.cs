using MediatR;
using Microsoft.EntityFrameworkCore;
using Mlsat.Models.Entities.DataSources;
using Mlsat.Models.Entities.DataSources.Parameters;
using Mlsat.Models.Entities.SpaceWeather;
using Mlsat.Services;
using Mlsat.Services.ModelsServices;
using Mlsat.Services.ModelsServices.Requests;
using Mlsat.Services.PythonServices;

namespace Mlsat.Features.DataSources.Command.CreateOnBasis;

public class CreateOnBasisCommand : IRequest
{
    public required int DataSourceId { get; set; }
    public required int ProjectId { get; set; }
    public required string Title { get; set; }
    public required bool IsDstLoaded { get; set; }
    public required bool IsNormalize { get; set; }
    public required bool IsLoadDst { get; set; }
    public required bool IsLoadKp { get; set; }
    public required bool IsLoadAp { get; set; }
    public required bool IsLoadWolf { get; set; }
    public required bool IsNaDropped { get; set; }
    public required string TimeColumn { get; set; }
    public required IReadOnlyCollection<string> Columns { get; set; }
}

public class CreateOnBasisCommandHandler : IRequestHandler<CreateOnBasisCommand>
{
    private readonly AppDbContext _context;
    private readonly ModelsService _modelsService;
    private readonly FileService _fileService;

    public CreateOnBasisCommandHandler(
        AppDbContext context,
        ModelsService modelsService,
        FileService fileService)
    {
        _context = context;
        _modelsService = modelsService;
        _fileService = fileService;
    }

    public async Task Handle(CreateOnBasisCommand request, CancellationToken cancellationToken)
    {
        var baseDataSource = await _context.DataSources
            .Where(d => d.Id == request.DataSourceId)
            .FirstAsync(cancellationToken);

        var project = await _context.Projects
            .Where(p => p.Id == request.ProjectId)
            .FirstAsync(cancellationToken);

        var dataSource = new DataSource(new CreateDataSourceParameters
        {
            Project = project,
            BaseDataSource = baseDataSource,
            Path = _fileService.CreateAndGetProjectDatasourceDirectory(project.Title) + $@"{request.Title}.csv",
            IsDstLoaded = request.IsDstLoaded,
            IsNormalize = request.IsNormalize,
            IsLoadDst = request.IsLoadDst,
            IsLoadKp = request.IsLoadKp,
            IsLoadAp = request.IsLoadAp,
            IsLoadWolf = request.IsLoadWolf,
            IsNaDropped = request.IsNaDropped,
            TimeColumn = request.TimeColumn,
            Columns = request.Columns,
            StartDate = baseDataSource.StartDate,
            EndDate = baseDataSource.EndDate
        });

        await _context.DataSources.AddAsync(dataSource, cancellationToken);

        await _modelsService.CreateOnBasis(new CreateOnBasisRequestDto
        {
            path = baseDataSource.Path,
            savePath = dataSource.Path,
            isNormalize = dataSource.IsNormalize,
            dropna = dataSource.IsNaDropped,
            timeColumn = dataSource.TimeColumn,
            columns = dataSource.Columns.Select(c => c.Title).ToList(),
            ap = dataSource.IsLoadAp
                ? await LoadSpaceWeather<Ap>(dataSource.StartDate, dataSource.EndDate)
                : null,
            kp = dataSource.IsLoadKp
                ? await LoadSpaceWeather<Kp>(dataSource.StartDate, dataSource.EndDate)
                : null,
            dst = dataSource.IsLoadDst
                ? await LoadSpaceWeather<Dst>(dataSource.StartDate, dataSource.EndDate)
                : null,
            wolf = dataSource.IsLoadWolf
                ? await LoadSpaceWeather<Wolf>(dataSource.StartDate, dataSource.EndDate)
                : null
        });

        await _context.SaveChangesAsync(cancellationToken);
    }

    private async Task<Dictionary<DateTime, decimal>> LoadSpaceWeather<T>(DateTime startDate, DateTime endDate)
        where T : class, ISpaceWeatherPoint
    {
        var points = await _context.Set<T>()
            .AsNoTracking()
            .Where(x => startDate <= x.Date && x.Date <= endDate)
            .ToDictionaryAsync(x => x.Date,
                x => x.Value!.Value);

        return points;
    }
}