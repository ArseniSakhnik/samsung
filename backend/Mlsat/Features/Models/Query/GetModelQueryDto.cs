using Mlsat.Models.Entities.Models.Lofs;
using Mlsat.Models.Enums;

namespace Mlsat.Features.Models.Query;

public class GetModelQueryDto
{
    public required List<ModelDto> Knn { get; set; } = new();
    public required List<ModelDto> Lof { get; set; } = new();
    public required List<ModelDto> IsolationForest { get; set; } = new();
    public required List<ModelDto> Gmm { get; set; } = new();
    public required List<ModelDto> Kde { get; set; } = new();
    public required List<ModelDto> Ocsvm { get; set; } = new();
    public required List<ModelDto> Dbscan { get; set; } = new();
    public required List<ModelDto> Optics { get; set; } = new();
    public required List<ModelDto> Gad { get; set; } = new();
    public required List<ModelDto> Kmad { get; set; } = new();
    public required List<ModelDto> Had { get; set; } = new();
    public required List<ModelDto> Sad { get; set; } = new();
    public required List<ModelDto> Aad { get; set; } = new();
    public required List<ModelDto> Pcad { get; set; } = new();
    public required List<ModelDto> Svd { get; set; } = new();
    public required List<ModelDto> Chad { get; set; } = new();
    public required List<ModelDto> Mvad { get; set; } = new();
    public required List<ModelDto> Hsad { get; set; } = new();
    public required List<ModelDto> FeatureBagging { get; set; } = new();
    public required List<ModelDto> Ife { get; set; } = new();
    public required List<ModelDto> Admc { get; set; } = new();
    public required List<ModelDto> Autoencoder { get; set; } = new();
    public required List<ModelDto> Ffnn { get; set; } = new();
    public required List<ModelDto> Gan { get; set; } = new();
    public required List<ModelDto> Rnn { get; set; } = new();
    public required List<ModelDto> Cnn { get; set; } = new();

    public class ModelDto
    {
        public required int Id { get; set; }
        public required int ProjectId { get; set; }
        public required int DataSourceId { get; set; }
        public required ModelTypeEnum ModelType { get; set; }
        public required int Version { get; set; }
        public required string Path { get; set; } = default!;
        public required IReadOnlyCollection<string> ModelColumns { get; set; } = new List<string>();
        public required IReadOnlyCollection<string> SpaceWeatherColumns { get; set; } = new List<string>();
        public required IReadOnlyCollection<string> Graphics { get; set; }
    }
}