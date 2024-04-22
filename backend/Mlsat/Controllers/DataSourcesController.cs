using Microsoft.AspNetCore.Mvc;
using Mlsat.Features.DataSources.Command.CreateOnBasis;
using Mlsat.Features.DataSources.Command.LoadDataSource;
using Mlsat.Features.DataSources.Query.GetDatasetColumns;
using Mlsat.Features.DataSources.Query.GetDatasource;

namespace Mlsat.Controllers;

public class DataSourcesController : BaseController
{
    [HttpGet("{datasourceId:int}")]
    public async Task<ActionResult<GetDatasourceQueryResultDto>> GetDatasource(
        int datasourceId,
        CancellationToken cancellationToken)
    {
        return Ok(await Mediator.Send(new GetDatasourceQuery
        {
            DatasourceId = datasourceId
        }, cancellationToken));
    }

    [HttpPost("get-columns")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> GetDatasetColumns([FromForm] GetDatasetColumnsQuery query)
    {
        return Ok(await Mediator.Send(query));
    }

    [HttpPost]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> LoadDataset(
        [FromQuery] int projectId,
        [FromQuery] string timeColumn,
        [FromQuery] string datasourceTitle,
        [FromForm] LoadDataSourceCommand.DatasetModelDto dataset)
    {
        await Mediator.Send(new LoadDataSourceCommand
        {
            ProjectId = projectId,
            DataSourceDto = dataset,
            TimeColumn = timeColumn,
            DataSourceTitle = datasourceTitle
        });

        return Ok();
    }

    [HttpPost("create-on-basis")]
    public async Task<IActionResult> CreateOnBasis(CreateOnBasisCommand command)
    {
        await Mediator.Send(command);
        return Ok();
    }
}