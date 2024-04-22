using MediatR;
using Mlsat.Models;
using Mlsat.Models.Entities.Projects;
using Mlsat.Models.Parameters;
using Mlsat.Services;

namespace Mlsat.Features.Projects.Command.CreateProject;

public class CreateProjectCommand : IRequest
{
    public required string Title { get; set; }
}

public class CreateProjectCommandHandler : IRequestHandler<CreateProjectCommand>
{
    private readonly AppDbContext _context;

    public CreateProjectCommandHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task Handle(CreateProjectCommand request, CancellationToken cancellationToken)
    {
        var project = new Project(new CreateProjectParameters
        {
            Title = request.Title
        });

        await _context.Projects.AddAsync(project, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);
    }
}