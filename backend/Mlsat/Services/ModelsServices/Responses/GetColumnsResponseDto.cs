namespace Mlsat.Services.PythonServices.Requests;

public class GetColumnsResponseDto
{
    public IReadOnlyCollection<string> Columns { get; set; } = new List<string>();
}