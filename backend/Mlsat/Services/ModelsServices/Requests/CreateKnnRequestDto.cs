namespace Mlsat.Services.ModelsServices.Requests;

public class CreateKnnRequestDto
{
    public required string datasetPath { get; set; } = default!;
    public required string savePath { get; set; } = default!;
    public required string timeColumn { get; set; } = default!;
    public required int nNeighbors { get; set; } = default!;
    public required string algorithm { get; set; } = default;
    public required IEnumerable<string> spaceWeatherColumns { get; set; } = default!;
    public required IEnumerable<string> columns { get; set; } = default!;
}