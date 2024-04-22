using Mlsat.Models.Entities.Models.BaseModels;
using Mlsat.Models.Entities.Models.BaseModels.Parameters;

namespace Mlsat.Models.Entities.Models.Gans;

public class GanAutoencoder : Model
{
    private GanAutoencoder() {}

    public GanAutoencoder(CreateBaseModelParameters parameters) : base(parameters)
    {
    }
}