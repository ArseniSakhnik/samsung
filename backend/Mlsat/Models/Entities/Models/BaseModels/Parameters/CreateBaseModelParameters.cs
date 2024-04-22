using Mlsat.Models.Entities.DataSources;
using Mlsat.Models.Entities.Projects;
using Mlsat.Models.Enums;

namespace Mlsat.Models.Entities.Models.BaseModels.Parameters;

public class CreateBaseModelParameters
{
    public required Project Project { get; set; }
    public required ModelTypeEnum ModelTypeType { get; set; }
    public required DataSource DataSource { get; set; }
    public required int Version { get; set; }
    public required string Path { get; set; }
    public required IEnumerable<string> ModelColumns { get; set; }
    public required IEnumerable<string> SpaceWeatherColumns { get; set; }
}