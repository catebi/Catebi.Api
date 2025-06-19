namespace Catebi.Api.Domain.Features.AdoptionBot.Converters;

public static class CatPaymentConverter
{
    public static CatPaymentDto ToDto(AtCatPayment payment) => new()
    {
        RecordId = payment.RecordId,
        CatRecordId = payment.CatRecordId,
        OwnerRecordId = payment.OwnerRecordId,
        Proof = payment.Proof,
        Status = payment.StatusValue,
        CreatedAt = payment.CreatedAt,
        CatName = payment.CatName
    };
} 