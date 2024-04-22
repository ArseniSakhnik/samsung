using MediatR;
using Microsoft.EntityFrameworkCore;
using Mlsat.Services;

namespace Mlsat.Features.Projects.Query.GetProject;

public class GetProjectQuery : IRequest<object>
{
    public int ProjectId { get; set; }
}

public class GetProjectQueryHandler : IRequestHandler<GetProjectQuery, object>
{
    private readonly AppDbContext _context;
    private readonly FileService _fileService;

    public GetProjectQueryHandler(AppDbContext context, FileService fileService)
    {
        _context = context;
        _fileService = fileService;
    }

    public async Task<object> Handle(GetProjectQuery request, CancellationToken cancellationToken)
    {
        var injection = ";DROP TABLE test;";

        var project = await _context.Projects
            .Where(p => p.Title == injection)
            .ToListAsync(cancellationToken);
        
        return await _context.Projects
            .Select(p => new
            {
                p.Id,
                p.Title,
                DataSources = p.DataSources.Select(d => new
                {
                    d.Id,
                    d.ProjectId,
                    d.BaseDataSourceId,
                    d.Path,
                    d.IsDstLoaded,
                    d.IsNormalize,
                    d.IsLoadDst,
                    d.IsLoadKp,
                    d.IsLoadAp,
                    d.IsLoadWolf,
                    d.IsNaDropped,
                    d.TimeColumn,
                    Columns = d.Columns.Select(c => c.Title),
                }),
                Models = p.Models.Select(m => new
                {
                    m.Id,
                    m.ProjectId,
                    m.DataSourceId,
                    m.Version,
                    m.Path,
                    ModelColumns = m.ModelColumns.Select(c => c.Title),
                    SpaceWeatherColumns = m.SpaceWeatherColumns.Select(s => s.Title),
                    Graphics = _fileService.GetModelGraphics(m.Path)
                }),
            })
            .Where(p => p.Id == request.ProjectId)
            .FirstAsync(cancellationToken);
    }
}