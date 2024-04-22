using Microsoft.AspNetCore.Mvc;
using Mlsat.Features.Models.Command;
using Mlsat.Features.Models.Query;

namespace Mlsat.Controllers;

public class ModelsController : BaseController
{
    [HttpPost]
    public async Task<IActionResult> Run(RunModelCommand command, CancellationToken cancellationToken)
    {
        await Mediator.Send(command, cancellationToken);
        return Ok();
    }

    [HttpGet]
    public async Task<IActionResult> GetModels()
    {
        return Ok(await Mediator.Send(new GetModelsQuery()));
    }

    [HttpGet("{modelId:int}")]
    public async Task<IActionResult> GetModel(int modelId)
    {
        return Ok(await Mediator.Send(new GetModelQuery
        {
            ProjectId = modelId
        }));
    }
}