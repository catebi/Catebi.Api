namespace Catebi.Api.Domain.Features.AdoptionBot.ViewModels;

public class DashboardInfoDto
{
    public List<DashboardInfoItemDto> Items { get; set; } = [];
}

public class DashboardInfoItemDto
{
    public required string Type { get; set; }
    public int Count { get; set; }
    public int? ToConfirmCount { get; set; }
}

public enum DashboardItemType
{
    Users,
    Cats,
    Payments,
    Events,
    Catbook
}
