namespace Catebi.Api.Domain.Features.AdoptionBot.Converters;

public static class CatConverter
{
    public static CatDto ToDto(AtCat cat, int paidAccountPrice = 0, string? recordId = null) => new()
    {
        CatId = cat.CatId,
        RecordId = recordId ?? cat.RecordId,
        OwnerRecordId = cat.OwnerRecordId ?? string.Empty,
        OwnerTelegram = cat.OwnerTelegram ?? string.Empty,
        Name = cat.Name,
        Sex = cat.SexValue,
        DateOfBirth = cat.DateOfBirth.ToString("yyyy-MM-dd"),
        MainPhoto = cat.MainPhoto?.Select(p => new AttachmentDto { Id = p.Id, Url = p.Url, Filename = p.FileName }).FirstOrDefault(),
        Photos = cat.Photos?.Select(p => new AttachmentDto { Id = p.Id, Url = p.Url, Filename = p.FileName }).ToList(),
        Status = cat.StatusValue,
        IsVaccinatedComplex = cat.IsVaccinatedComplex,
        IsVaccinatedRabies = cat.IsVaccinatedRabies,
        OwnerNotes = cat.OwnerNotes,
        AdoptionComment = cat.AdoptionComment,
        OwnerIsVolunteer = cat.OwnerIsVolunteer,
        PaidAccountPrice = paidAccountPrice,
        ConfirmedPaymentAccount = cat.AccountPaymentStatus == Enums.CatPaymentStatuses.Confirmed,
        Created = cat.Created.ToString("yyyy-MM-dd HH:mm:ss")
    };
}
