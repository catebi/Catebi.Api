using Catebi.Api.Domain.Features.AdoptionBot.Enums;
using System.Text.Json.Serialization;

namespace Catebi.Api.Domain.Features.AdoptionBot.Models;

public class AtCat
{
    #region At fields
    public int CatId { get; set; }
    public string Name { get; set; }
    public string CatbookUrl { get; set; }


    [JsonPropertyName("OwnerName")]
    public string[] OwnerNameValue { get; set; }

    [JsonPropertyName("AccountPaymentRecordId")]
    public string[] AccountPaymentRecordIdValue { get; set; }

    [JsonPropertyName("AccountPaymentType")]
    public string[] AccountPaymentTypeValue { get; set; }

    [JsonPropertyName("AccountPaymentStatus")]
    public string[] AccountPaymentStatusValue { get; set; }

    [JsonPropertyName("OwnerTelegramChatId")]
    public int[] OwnerTelegramChatIdValue { get; set; }

    [JsonPropertyName("Status")]
    public string StatusValue { get; set; }

    [JsonPropertyName("OwnerRole")]
    public string[] OwnerRoleValue { get; set; }

    #endregion

    [JsonIgnore]
    public string OwnerName => OwnerNameValue.First();

    [JsonIgnore]
    public int OwnerTelegramChatId => OwnerTelegramChatIdValue.FirstOrDefault();

    [JsonIgnore]
    public CatStatuses Status => Enum.Parse<CatStatuses>(StatusValue);

    [JsonIgnore]
    public UserRoles OwnerRole => Enum.Parse<UserRoles>(OwnerRoleValue.First());

    [JsonIgnore]
    public string? AccountPaymentRecordId => AccountPaymentRecordIdValue.FirstOrDefault();

    [JsonIgnore]
    public PaymentOptionTypes? AccountPaymentType => Enum.TryParse<PaymentOptionTypes>(AccountPaymentTypeValue[0], out var paymentType) ? paymentType : null;

    [JsonIgnore]
    public CatPaymentStatuses? AccountPaymentStatus => Enum.TryParse<CatPaymentStatuses>(AccountPaymentStatusValue[0], out var paymentStatus) ? paymentStatus : null;
}
