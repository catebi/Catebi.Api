namespace Catebi.Api.Domain.Features.AdoptionBot.Converters;

public static class CatbookConverter
{
    public static CatbookInfoDto ToDto(AtCatbook catbook, string? recordId = null) => new()
    {
        RecordId = recordId ?? catbook.RecordId,
        CatbookId = catbook.CatbookId,
        CatRecordId = catbook.CatRecordId,
        CatName = catbook.CatName,
        Descr = catbook.Descr,
        Color = catbook.Color,
        Aliases = catbook.Aliases,
        MediaLink = catbook.MediaLink,
        HealthNotes = catbook.HealthNotes,
        HasPassport = catbook.HasPassport,
        CharacterNotes = catbook.CharacterNotes,
        HistoryNotes = catbook.HistoryNotes,
        Location = catbook.Location,
        DeliveryAvailable = catbook.DeliveryAvailable,
        DeliveryNotes = catbook.DeliveryNotes,
        ContactTg = catbook.ContactTg,
        Status = catbook.StatusValue,
        CatbookLink = catbook.CatbookLink,
        CatbookPostId = catbook.CatbookPostId,
        Created = catbook.Created?.ToString("yyyy-MM-dd HH:mm:ss"),
        Updated = catbook.Updated?.ToString("yyyy-MM-dd HH:mm:ss")
    };
}
