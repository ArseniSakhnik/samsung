using MediatR;
using Microsoft.EntityFrameworkCore;
using Mlsat.Models.Entities.Models.BaseModels;
using Mlsat.Models.Entities.Models.BaseModels.Parameters;
using Mlsat.Models.Enums;
using Mlsat.Services;
using Mlsat.Services.ModelsServices;
using Mlsat.Services.ModelsServices.Requests;

namespace Mlsat.Features.Models.Command;

public class RunModelCommand : IRequest
{
    public required int ProjectId { get; set; }
    public required ModelTypeEnum ModelType { get; set; }
    public required int DataSourceId { get; set; }
    public required IReadOnlyCollection<string> Columns { get; set; }
    public required IReadOnlyCollection<string> SpaceWeatherColumns { get; set; }
}

public class RunModelCommandHandler : IRequestHandler<RunModelCommand>
{
    private readonly AppDbContext _context;
    private readonly FileService _fileService;
    private readonly ModelsService _modelsService;

    public RunModelCommandHandler(AppDbContext context, FileService fileService, ModelsService modelsService)
    {
        _context = context;
        _fileService = fileService;
        _modelsService = modelsService;
    }

    public async Task Handle(RunModelCommand request, CancellationToken cancellationToken)
    {
        var project = await _context.Projects
            .Where(p => p.Id == request.ProjectId)
            .FirstAsync(cancellationToken);

        var dataSource = await _context.DataSources
            .Where(d => d.Id == request.DataSourceId)
            .FirstAsync(cancellationToken);

        var prevModel = await _context.Models
            .Where(m => m.ModelType == request.ModelType)
            .OrderBy(k => k.Id)
            .LastOrDefaultAsync(cancellationToken);

        var columns = request.Columns
            .Except(new List<string>() { dataSource.TimeColumn });
        
        var version = prevModel?.Version ?? 0;

        var model = new Model(new CreateBaseModelParameters
        {
            Project = project,
            ModelTypeType = request.ModelType,
            DataSource = dataSource,
            Version = ++version,
            Path = _fileService.CreateAndGetModelDirectory(project.Title, request.ModelType.ToString(), version),
            ModelColumns = columns,
            SpaceWeatherColumns = request.SpaceWeatherColumns
        });

        await _context.Models.AddAsync(model, cancellationToken);

        await _modelsService.RunModel(new CreateModelRequestDto
        {
            datasetPath = dataSource.Path,
            savePath = model.Path,
            timeColumn = dataSource.TimeColumn,
            spaceWeatherColumns = request.SpaceWeatherColumns,
            columns = request.Columns,
            modelType = (int)model.ModelType
        });

        await _context.SaveChangesAsync(cancellationToken);
    }
}