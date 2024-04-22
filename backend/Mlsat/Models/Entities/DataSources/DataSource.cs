using Mlsat.Models.Entities.Columns;
using Mlsat.Models.Entities.DataSources.Parameters;
using Mlsat.Models.Entities.Projects;

namespace Mlsat.Models.Entities.DataSources;

public class DataSource
{
    public int Id { get; private set; }
    public int ProjectId { get; private set; }
    public Project Project { get; private set; }
    public int? BaseDataSourceId { get; private set; }
    public DataSource? BaseDataSource { get; private set; }
    public string Path { get; private set; }
    public bool IsDstLoaded { get; private set; }
    public bool IsNormalize { get; private set; }
    public bool IsLoadDst { get; private set; }
    public bool IsLoadKp { get; private set; }
    public bool IsLoadAp { get; private set; }
    public bool IsLoadWolf { get; private set; }
    public bool IsNaDropped { get; private set; }
    public string TimeColumn { get; private set; }
    public IReadOnlyCollection<Column> Columns { get; private set; }

    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }


    private DataSource()
    {
        Id = 0;
    }

    public DataSource(CreateDataSourceParameters parameters) : this()
    {
        ProjectId = parameters.Project.Id;
        Project = parameters.Project;
        BaseDataSource = parameters.BaseDataSource;
        BaseDataSourceId = parameters.BaseDataSource?.Id;
        Path = parameters.Path;
        IsDstLoaded = parameters.IsDstLoaded;
        IsNormalize = parameters.IsNormalize;
        IsLoadDst = parameters.IsLoadDst;
        IsLoadKp = parameters.IsLoadKp;
        IsLoadAp = parameters.IsLoadAp;
        IsLoadWolf = parameters.IsLoadWolf;
        IsNaDropped = parameters.IsNaDropped;
        TimeColumn = parameters.TimeColumn;
        Columns = parameters.Columns
            .Select(c => new Column
            {
                Title = c
            })
            .ToList();
        StartDate = parameters.StartDate;
        EndDate = parameters.EndDate;
    }
}