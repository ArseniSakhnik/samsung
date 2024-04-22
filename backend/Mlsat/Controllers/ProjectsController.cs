using Microsoft.AspNetCore.Mvc;
using Mlsat.Features.Projects.Command.CreateProject;
using Mlsat.Features.Projects.Query.GetProject;
using Mlsat.Features.Projects.Query.GetProjects;

namespace Mlsat.Controllers;

public class ProjectsController : BaseController
{
    [HttpGet]
    public async Task<IActionResult> GetProjects()
    {
        return Ok(await Mediator.Send(new GetProjectsQuery()));
    }

    [HttpGet("{projectId:int}")]
    public async Task<IActionResult> GetProject(int projectId)
    {
        return Ok(await Mediator.Send(new GetProjectQuery
        {
            ProjectId = projectId
        }));
    }

    [HttpPost]
    public async Task<IActionResult> CreateProject(CreateProjectCommand command)
    {
        await Mediator.Send(command);
        return Ok();
    }
}