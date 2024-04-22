namespace Mlsat.Services.ModelsServices.Requests;

public class GetDatesRequestDto
{
    public required string Path { get; set; }
    public required string TimeColumn { get; set; }
}