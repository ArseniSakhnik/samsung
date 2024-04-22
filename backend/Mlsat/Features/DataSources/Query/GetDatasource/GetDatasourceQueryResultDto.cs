namespace Mlsat.Features.DataSources.Query.GetDatasource;

public class GetDatasourceQueryResultDto
{
    public required int Id { get; set; }
    public required int ProjectId { get; set; }  
    public required int? BaseDataSourceId { get; set; }
    public required string Path { get; set; }
    public required bool IsDstLoaded { get; set; }
    public required bool IsNormalize { get; set; }
    public required bool IsLoadDst { get; set; }
    public required bool IsLoadKp { get; set; }
    public required bool IsLoadAp { get; set; }
    public required bool IsLoadWolf { get; set; }
    public required bool IsNaDropped { get; set; }
    public required string TimeColumn { get; set; }
    public required IEnumerable<ColumnDto> Columns { get; set; }

    public class ColumnDto
    {
        public required string Title { get; set; }
    }
}