using MediatR;
using Microsoft.AspNetCore.Mvc;
using Mlsat.Features.SpaceWeather.Command.SyncSpaceWeather;

namespace Mlsat.Controllers;

public class SpaceWeatherController : BaseController
{
    [HttpPost]
    public async Task<IActionResult> SyncSpaceWeather(
        [FromQuery] SyncSpaceWeatherCommand command,
        CancellationToken cancellationToken)
    {
        await Mediator.Send(command, cancellationToken);
        return Ok();
    }
}