namespace Catebi.Api.Domain.Features.AdoptionBot.Converters;

public static class CatPaymentConverter
{
    public static CatPaymentDto ToDto(AtCatPayment payment) => new()
    {
        RecordId = payment.RecordId ?? string.Empty,
        CatRecordId = payment.CatRecordId ?? string.Empty,
        CatName = payment.CatName ?? string.Empty,
        OwnerName = payment.OwnerName ?? string.Empty,
        OwnerTelegram = payment.OwnerTelegram ?? string.Empty,
        Price = payment.Price,
        Proof = payment.Proof?.Select(p => new AttachmentDto { Id = p.Id, Url = p.Url, Filename = p.FileName }).FirstOrDefault(),
        Status = payment.StatusValue,
        Created = payment.Created.ToString("yyyy-MM-dd HH:mm:ss")
    };
}
