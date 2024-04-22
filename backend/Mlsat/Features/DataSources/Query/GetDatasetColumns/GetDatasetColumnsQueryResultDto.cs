namespace Mlsat.Features.DataSources.Query.GetDatasetColumns;

public class GetDatasetColumnsQueryResultDto
{
    public required IReadOnlyCollection<string> Columns { get; set; }
}