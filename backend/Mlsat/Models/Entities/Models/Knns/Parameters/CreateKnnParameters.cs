using Mlsat.Models.Entities.Models.BaseModels.Parameters;

namespace Mlsat.Models.Entities.Models.Knns.Parameters;

public class CreateKnnParameters : CreateBaseModelParameters
{
    public required int NNeighbors { get; set; }
    public required string Algorithm { get; set; }
    public required int Percentile { get; set; }
}