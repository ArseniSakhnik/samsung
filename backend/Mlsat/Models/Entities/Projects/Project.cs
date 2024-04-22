using Mlsat.Models.Entities.DataSources;
using Mlsat.Models.Entities.Models;
using Mlsat.Models.Entities.Models.BaseModels;
using Mlsat.Models.Parameters;

namespace Mlsat.Models.Entities.Projects;

public class Project
{
    public int Id { get; set; }
    public string Title { get; set; } = default!;
    public ICollection<DataSource> DataSources { get; set; } = new List<DataSource>();

    public IEnumerable<Model> Models { get; set; } = new List<Model>();

    private Project()
    {
        Id = 0;
    }

    public Project(CreateProjectParameters parameters) : this()
    {
        Title = parameters.Title;
    }
}