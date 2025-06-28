using Catebi.Api.Domain.Features.AdoptionBot.Enums;
using System.Text.Json.Serialization;

namespace Catebi.Api.Domain.Features.AdoptionBot.Models;

public class AtCatPayment
{
    public string? RecordId { get; set; }

    [JsonPropertyName("CatRecordId")]
    public string[] CatRecordIdValue { get; set; } = [];

    [JsonPropertyName("OwnerRecordId")]
    public string[] OwnerRecordIdValue { get; set; } = [];

    public AtAttachment[] Proof { get; set; } = [];

    public DateTime Created { get; set; }

    [JsonPropertyName("CatName")]
    public string[] CatNameValue { get; set; } = [];

    [JsonPropertyName("Status")]
    public string StatusValue { get; set; }

    [JsonPropertyName("PaymentType")]
    public string[] PaymentTypeValue { get; set; } = [];

    [JsonIgnore]
    public string? CatRecordId => CatRecordIdValue.FirstOrDefault();

    [JsonIgnore]
    public string? OwnerRecordId => OwnerRecordIdValue.FirstOrDefault();

    [JsonIgnore]
    public string? CatName => CatNameValue.FirstOrDefault();

    [JsonIgnore]
    public CatPaymentStatuses Status => Enum.Parse<CatPaymentStatuses>(StatusValue);

    [JsonIgnore]
    public PaymentOptionTypes? PaymentType => Enum.TryParse<PaymentOptionTypes>(PaymentTypeValue.FirstOrDefault(), out var paymentType) ? paymentType : null;
}
