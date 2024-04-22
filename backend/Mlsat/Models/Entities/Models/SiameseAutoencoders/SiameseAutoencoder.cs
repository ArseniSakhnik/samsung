using Mlsat.Models.Entities.Models.BaseModels;
using Mlsat.Models.Entities.Models.BaseModels.Parameters;

namespace Mlsat.Models.Entities.Models.SiameseAutoencoders;

public class SiameseAutoencoder : Model
{
    private SiameseAutoencoder() {}
    
    public SiameseAutoencoder(CreateBaseModelParameters parameters) : base(parameters)
    {
    }
}