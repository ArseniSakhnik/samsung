using Mlsat.Models.Entities.DataSources;
using Mlsat.Models.Entities.Models.BaseModels.Parameters;
using Mlsat.Models.Entities.Projects;
using Mlsat.Models.Enums;

namespace Mlsat.Models.Entities.Models.BaseModels;

public class Model
{
    public int Id { get; set; }
    public int ProjectId { get; set; }
    public ModelTypeEnum ModelType { get; set; }
    public Project Project { get; set; } = default!;
    public int DataSourceId { get; set; }
    public DataSource DataSource { get; set; } = default!;
    public int Version { get; set; }
    public string Path { get; set; } = default!;
    public IReadOnlyCollection<ModelColumn> ModelColumns { get; set; } = new List<ModelColumn>();
    public IReadOnlyCollection<SpaceWeatherColumn> SpaceWeatherColumns { get; set; } = new List<SpaceWeatherColumn>();

    protected Model()
    {
    }

    public Model(CreateBaseModelParameters parameters)
    {
        ProjectId = parameters.Project.Id;
        Project = parameters.Project;
        DataSourceId = parameters.DataSource.Id;
        DataSource = parameters.DataSource;
        Version = parameters.Version;
        Path = parameters.Path;
        ModelType = parameters.ModelTypeType;
        ModelColumns = parameters.ModelColumns.Select(c => new ModelColumn
        {
            Title = c
        }).ToList();
        SpaceWeatherColumns = parameters.SpaceWeatherColumns.Select(c => new SpaceWeatherColumn
        {
            Title = c
        }).ToList();
    }
}