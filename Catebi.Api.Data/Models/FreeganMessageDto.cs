namespace Catebi.Api.Data.Models;

public class FreeganMessageDto
{
    public string OriginalText { get; set; } = null!;
    public string LemmatizedText { get; set; } = null!;
    public string ChatLink { get; set; } = null!;
    public bool Accepted { get; set; }
}