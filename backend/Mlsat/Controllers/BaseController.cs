using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Mlsat.Controllers;

[ApiController]
[Route("api/[controller]")]
public abstract class BaseController : ControllerBase
{
    protected IMediator Mediator => HttpContext.RequestServices.GetService<IMediator>()
                                    ??
                                    throw new NullReferenceException();
}