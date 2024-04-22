using MediatR;
using Microsoft.EntityFrameworkCore;
using Mlsat.Services;

namespace Mlsat.Features.DataSources.Query.GetDatasource;

public class GetDatasourceQuery : IRequest<GetDatasourceQueryResultDto>
{
    public int DatasourceId { get; set; }
}

public class GetDatasourceQueryHandler : IRequestHandler<GetDatasourceQuery, GetDatasourceQueryResultDto>
{
    private readonly AppDbContext _context;

    public GetDatasourceQueryHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<GetDatasourceQueryResultDto> Handle(GetDatasourceQuery request,
        CancellationToken cancellationToken)
    {
        var datasource = await _context.DataSources
            .Include(d => d.Columns)
            .Where(d => d.Id == request.DatasourceId)
            .FirstAsync(cancellationToken);

        return new GetDatasourceQueryResultDto
        {
            Id = datasource.Id,
            ProjectId = datasource.ProjectId,
            BaseDataSourceId = datasource.BaseDataSourceId,
            Path = datasource.Path,
            IsDstLoaded = datasource.IsDstLoaded,
            IsNormalize = datasource.IsNormalize,
            IsLoadDst = datasource.IsLoadDst,
            IsLoadKp = datasource.IsLoadKp,
            IsLoadAp = datasource.IsLoadAp,
            IsLoadWolf = datasource.IsLoadWolf,
            IsNaDropped = datasource.IsNaDropped,
            TimeColumn = datasource.TimeColumn,
            Columns = datasource.Columns
                .Select(c => new GetDatasourceQueryResultDto.ColumnDto
                {
                    Title = c.Title
                }),
        };
    }
}