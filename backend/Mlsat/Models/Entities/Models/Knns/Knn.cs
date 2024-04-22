using Mlsat.Models.Entities.Models.BaseModels;
using Mlsat.Models.Entities.Models.BaseModels.Parameters;
using Mlsat.Models.Entities.Models.Knns.Parameters;

namespace Mlsat.Models.Entities.Models.Knns;

public class Knn : Model
{
    public int NNeighbors { get; set; }
    public string Algorithm { get; set; } = default!;
    public int Percentile { get; set; } = 95;

    public Knn() : base()
    {
    }

    public Knn(CreateKnnParameters parameters) : base(parameters)
    {
        NNeighbors = parameters.NNeighbors;
        Algorithm = parameters.Algorithm;
        Percentile = parameters.Percentile;
    }
}