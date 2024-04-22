namespace Mlsat.Services.ModelsServices.Requests;

public class CreateOnBasisRequestDto
{
    public required string path { get; set; }
    public required string savePath { get; set; }
    public required bool isNormalize { get; set; }
    public required bool dropna { get; set; }
    public required string timeColumn { get; set; }
    public required IReadOnlyCollection<string> columns { get; set; }
    public required Dictionary<DateTime, decimal>? dst { get; set; }
    public required Dictionary<DateTime, decimal>? kp { get; set; }
    public required Dictionary<DateTime, decimal>? ap { get; set; }
    public required Dictionary<DateTime, decimal>? wolf { get; set; }
}