using MediatR;
using Mlsat.Services;
using Mlsat.Services.ModelsServices;

namespace Mlsat.Features.DataSources.Query.GetDatasetColumns;

public class GetDatasetColumnsQuery : IRequest<GetDatasetColumnsQueryResultDto>
{
    public required IFormFile File { get; set; }
}

public class GetDatasetColumnsQueryHandler : IRequestHandler<GetDatasetColumnsQuery, GetDatasetColumnsQueryResultDto>
{
    private readonly FileService _fileService;
    private readonly ModelsService _modelsService;

    public GetDatasetColumnsQueryHandler(
        FileService fileService,
        ModelsService modelsService)
    {
        _fileService = fileService;
        _modelsService = modelsService;
    }

    public async Task<GetDatasetColumnsQueryResultDto> Handle(
        GetDatasetColumnsQuery request,
        CancellationToken cancellationToken)
    {
        var path = await _fileService.LoadDataSource(request.File);

        var columns = await _modelsService.GetColumns(path);

        return new GetDatasetColumnsQueryResultDto
        {
            Columns = columns
        };
    }
}