using Catebi.Api.Domain.Features.AdoptionBot.Enums;
using System.Text.Json.Serialization;

namespace Catebi.Api.Domain.Features.AdoptionBot.Models;

public class AtCatPayment
{
    public string? RecordId { get; set; }
    public string CatRecordId { get; set; }
    public string? OwnerRecordId { get; set; }
    public string Proof { get; set; }
    public DateTime? CreatedAt { get; set; }
    public string? CatName { get; set; }    
    public int CatId { get; set; }
    public int OwnerTelegramChatId { get; set; }
    public int TelegramChatId { get; set; }
    public string CatbookUrl { get; set; }

    [JsonPropertyName("Status")]
    public string StatusValue { get; set; }

    [JsonPropertyName("PaymentType")]
    public string PaymentTypeValue { get; set; }

    [JsonIgnore]
    public CatPaymentStatuses Status => Enum.Parse<CatPaymentStatuses>(StatusValue);

    [JsonIgnore]
    public PaymentOptionTypes PaymentType => Enum.Parse<PaymentOptionTypes>(PaymentTypeValue);
}
