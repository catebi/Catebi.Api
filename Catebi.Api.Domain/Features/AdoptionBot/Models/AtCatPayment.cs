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

    [JsonPropertyName("OwnerName")]
    public string[] OwnerNameValue { get; set; } = [];

    [JsonPropertyName("OwnerTelegram")]
    public string[] OwnerTelegramValue { get; set; } = [];

    [JsonPropertyName("Price")]
    public int[] PriceValue { get; set; } = [];

    public AtAttachment[] Proof { get; set; } = [];

    public DateTime Created { get; set; }

    [JsonPropertyName("CatName")]
    public string[] CatNameValue { get; set; } = [];

    [JsonPropertyName("Status")]
    public string StatusValue { get; set; } = string.Empty;

    [JsonPropertyName("PaymentType")]
    public string[] PaymentTypeValue { get; set; } = [];

    [JsonIgnore]
    public string? CatRecordId => CatRecordIdValue.FirstOrDefault();

    [JsonIgnore]
    public string? OwnerRecordId => OwnerRecordIdValue.FirstOrDefault();

    [JsonIgnore]
    public string? OwnerName => OwnerNameValue.FirstOrDefault();

    [JsonIgnore]
    public string? OwnerTelegram => OwnerTelegramValue.FirstOrDefault();

    [JsonIgnore]
    public int? Price => PriceValue.FirstOrDefault();

    [JsonIgnore]
    public string? CatName => CatNameValue.FirstOrDefault();

    [JsonIgnore]
    public CatPaymentStatuses Status => Enum.Parse<CatPaymentStatuses>(StatusValue);

    [JsonIgnore]
    public PaymentOptionTypes? PaymentType => Enum.TryParse<PaymentOptionTypes>(PaymentTypeValue.FirstOrDefault(), out var paymentType) ? paymentType : null;
}
