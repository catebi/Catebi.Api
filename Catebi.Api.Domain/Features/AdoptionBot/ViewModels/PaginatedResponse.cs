namespace Catebi.Api.Domain.Features.AdoptionBot.ViewModels;

public class PaginatedResponse<T>
{
    public IEnumerable<T> Records { get; set; } = new List<T>();
    public string? NextPageOffset { get; set; }
    public bool HasNextPage => !string.IsNullOrEmpty(NextPageOffset);
    public int PageSize { get; set; }
    public int RecordCount => Records.Count();
} 