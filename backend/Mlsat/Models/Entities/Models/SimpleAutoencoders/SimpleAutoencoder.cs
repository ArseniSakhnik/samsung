using Mlsat.Models.Entities.Models.BaseModels;
using Mlsat.Models.Entities.Models.BaseModels.Parameters;

namespace Mlsat.Models.Entities.Models.SimpleAutoencoders;

public class SimpleAutoencoder : Model
{
    private SimpleAutoencoder()
    {
    }

    public SimpleAutoencoder(CreateBaseModelParameters parameters) : base(parameters)
    {
    }
}