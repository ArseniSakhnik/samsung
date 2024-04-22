using MediatR;
using Microsoft.EntityFrameworkCore;
using Mlsat.Services;

namespace Mlsat.Features.Projects.Query.GetProjects;

public class GetProjectsQuery : IRequest<object>
{
}

public class GetProjectsQueryHandler : IRequestHandler<GetProjectsQuery, object>
{
    private readonly AppDbContext _context;
    private readonly FileService _fileService;

    public GetProjectsQueryHandler(AppDbContext context, FileService fileService)
    {
        _context = context;
        _fileService = fileService;
    }

    public async Task<object> Handle(GetProjectsQuery request, CancellationToken cancellationToken)
    {
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
                })
            })
            .ToListAsync(cancellationToken: cancellationToken);
    }
}