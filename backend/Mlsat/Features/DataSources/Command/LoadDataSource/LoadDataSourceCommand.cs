using MediatR;
using Microsoft.EntityFrameworkCore;
using Mlsat.Models.Entities.DataSources;
using Mlsat.Models.Entities.DataSources.Parameters;
using Mlsat.Services;
using Mlsat.Services.ModelsServices;
using Mlsat.Services.ModelsServices.Requests;
using Mlsat.Services.PythonServices;

namespace Mlsat.Features.DataSources.Command.LoadDataSource;

public class LoadDataSourceCommand : IRequest
{
    public required int ProjectId { get; set; }
    public required string TimeColumn { get; set; }
    public required string DataSourceTitle { get; set; }
    public required DatasetModelDto DataSourceDto { get; set; }

    public class DatasetModelDto
    {
        public required IFormFile File { get; set; }
    }
}

public class LoadDataSourceCommandHandler : IRequestHandler<LoadDataSourceCommand>
{
    private readonly AppDbContext _context;
    private readonly FileService _fileService;
    private readonly ModelsService _modelsService;

    public LoadDataSourceCommandHandler(
        AppDbContext context,
        FileService fileService,
        ModelsService modelsService)
    {
        _context = context;
        _fileService = fileService;
        _modelsService = modelsService;
    }

    public async Task Handle(LoadDataSourceCommand request, CancellationToken cancellationToken)
    {
        var project = await _context.Projects
            .Where(p => p.Id == request.ProjectId)
            .FirstAsync(cancellationToken);

        var dataSourcePath = await _fileService
            .LoadDataSource(project.Title, request.DataSourceTitle, request.DataSourceDto.File);

        var columns = await _modelsService.GetColumns(dataSourcePath);

        var dates = await _modelsService.GetDates(new GetDatesRequestDto
        {
            Path = dataSourcePath,
            TimeColumn = request.TimeColumn
        });

        var dataSource = new DataSource(new CreateDataSourceParameters
        {
            Project = project,
            Path = dataSourcePath,
            BaseDataSource = null,
            IsDstLoaded = false,
            IsNormalize = false,
            IsLoadDst = false,
            IsLoadKp = false,
            IsLoadAp = false,
            IsLoadWolf = false,
            IsNaDropped = false,
            TimeColumn = request.TimeColumn,
            Columns = columns,
            StartDate = dates.StartDate,
            EndDate = dates.EndDate
        });

        await _context.DataSources.AddAsync(dataSource, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }
}