using Mlsat.Models.Entities.Models.BaseModels;
using Mlsat.Models.Entities.Models.BaseModels.Parameters;

namespace Mlsat.Models.Entities.Models.Lofs;

public class Lof : Model
{
    public required int NNeighbors { get; set; }
    public required decimal Contamination { get; set; }
    
    private Lof() {}

    public Lof(CreateBaseModelParameters parameters) : base(parameters)
    {
    }
}