namespace Catebi.Api.Domain.Features.AdoptionBot.Converters;

public static class CatConverter
{
    public static CatDto ToDto(AtCat cat, string? recordId = null) => new()
    {
        CatId = cat.CatId,
        RecordId = recordId ?? cat.RecordId,
        OwnerRecordId = cat.OwnerRecordId ?? string.Empty,
        Name = cat.Name,
        DateOfBirth = cat.DateOfBirth.ToString("yyyy-MM-dd"),
        MainPhoto = cat.MainPhoto?.Select(p => new AttachmentDto { Id = p.Id, Url = p.Url, Filename = p.FileName }).FirstOrDefault(),
        Photos = cat.Photos?.Select(p => new AttachmentDto { Id = p.Id, Url = p.Url, Filename = p.FileName }).ToList(),
        Status = cat.StatusValue,
        IsVaccinatedComplex = cat.IsVaccinatedComplex,
        IsVaccinatedRabies = cat.IsVaccinatedRabies,
        OwnerNotes = cat.OwnerNotes
    };
} 