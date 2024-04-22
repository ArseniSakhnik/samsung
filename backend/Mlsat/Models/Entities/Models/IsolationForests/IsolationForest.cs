using Mlsat.Models.Entities.Models.BaseModels;
using Mlsat.Models.Entities.Models.BaseModels.Parameters;

namespace Mlsat.Models.Entities.Models.IsolationForests;

public class IsolationForest : Model
{
    public required decimal Contamination { get; set; }

    private IsolationForest()
    {
    }

    public IsolationForest(CreateBaseModelParameters parameters) : base(parameters)
    {
    }
}