namespace Catebi.Api.Domain.Features.AdoptionBot.Converters;

public static class EventConverter
{
    public static EventDto ToDto(AtEvent ev, string? recordId = null) => new()
    {
        EventId = ev.EventId,
        RecordId = recordId ?? ev.RecordId,
        Name = ev.Name,
        Description = ev.Description,
        When = ev.When.ToString("yyyy-MM-dd"),
        Where = ev.Where,
        Created = ev.Created?.ToString("yyyy-MM-dd"),
        Cats = ev.Cats?.ToList(),
        PaidSlotCount = ev.PaidSlotCount,
        FreeSlotCount = ev.FreeSlotCount,
        RegisteredCatCount = ev.RegisteredCatCount,
        Poster = ev.Poster?.Select(p => new AttachmentDto { Id = p.Id, Url = p.Url, Filename = p.FileName }).ToList(),
        Status = ev.StatusValue
    };

    public static AtEvent ToAtEvent(EventDto dto) => new()
    {
        RecordId = dto.RecordId,
        EventId = dto.EventId ?? 0,
        Name = dto.Name,
        Description = dto.Description,
        When = DateTime.TryParse(dto.When, out var when) ? when : default,
        Where = dto.Where,
        Created = string.IsNullOrEmpty(dto.Created) ? null : DateTime.Parse(dto.Created),
        Cats = dto.Cats?.ToArray() ?? [],
        PaidSlotCount = dto.PaidSlotCount,
        FreeSlotCount = dto.FreeSlotCount,
        RegisteredCatCount = dto.RegisteredCatCount,
        Poster = dto.Poster?.Select(p => new AtAttachment { Id = p.Id ?? string.Empty, Url = p.Url ?? string.Empty, FileName = p.Filename ?? string.Empty }).ToArray() ?? [],
        StatusValue = dto.Status
    };
}
