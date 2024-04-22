using Mlsat.Models.Entities.Projects;

namespace Mlsat.Models.Entities.DataSources.Parameters;

public class CreateDataSourceParameters
{
    public required Project Project { get; set; } = default!;
    public required DataSource? BaseDataSource { get; set; }
    public required string Path { get; set; }
    public required bool IsDstLoaded { get; set; }
    public required bool IsNormalize { get; set; }
    public required bool IsLoadDst { get; set; }
    public required bool IsLoadKp { get; set; }
    public required bool IsLoadAp { get; set; }
    public required bool IsLoadWolf { get; set; }
    public required bool IsNaDropped { get; set; }
    public required string TimeColumn { get; set; }
    public required IReadOnlyCollection<string> Columns { get; set; }
    public required DateTime StartDate { get; set; }
    public required DateTime EndDate { get; set; }
}