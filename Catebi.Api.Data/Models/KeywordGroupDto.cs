namespace Catebi.Api.Data.Models;

public class KeywordGroupDto
{
    public string Name { get; set; }
    public List<string> Keywords { get; set; }
    public List<string> IncludeKeywords { get; set; }
    public List<string> ExcludeKeywords { get; set; }
}