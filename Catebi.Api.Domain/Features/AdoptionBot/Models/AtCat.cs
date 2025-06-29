using Catebi.Api.Domain.Features.AdoptionBot.Enums;
using System.Text.Json.Serialization;

namespace Catebi.Api.Domain.Features.AdoptionBot.Models;

public class AtCat
{
    public string? RecordId { get; set; }
    public int CatId { get; set; }

    [JsonPropertyName("OwnerRecordId")]
    public string[] OwnerRecordIdValue { get; set; } = [];
    public string Name { get; set; }
    public DateTime DateOfBirth { get; set; }
    public AtAttachment[] MainPhoto { get; set; } = [];
    public AtAttachment[] Photos { get; set; } = [];

    [JsonPropertyName("Status")]
    public string StatusValue { get; set; }

    [JsonPropertyName("OwnerName")]
    public string[] OwnerNameValue { get; set; } = [];

    [JsonPropertyName("OwnerIsVolunteer")]
    public bool?[] OwnerIsVolunteerValue { get; set; } = [];

    [JsonPropertyName("AccountPaymentRecordId")]
    public string[] AccountPaymentRecordIdValue { get; set; } = [];

    [JsonPropertyName("AccountPaymentType")]
    public string[] AccountPaymentTypeValue { get; set; } = [];

    [JsonPropertyName("AccountPaymentStatus")]
    public string[] AccountPaymentStatusValue { get; set; } = [];

    [JsonPropertyName("OwnerTelegram")]
    public string[] OwnerTelegramValue { get; set; } = [];

    [JsonPropertyName("OwnerTelegramChatId")]
    public long[] OwnerTelegramChatIdValue { get; set; } = [];

    [JsonPropertyName("OwnerRole")]
    public string[] OwnerRoleValue { get; set; } = [];

    [JsonPropertyName("IsVaccinatedComplex")]
    public bool? IsVaccinatedComplex { get; set; }

    [JsonPropertyName("IsVaccinatedRabies")]
    public bool? IsVaccinatedRabies { get; set; }

    [JsonPropertyName("OwnerNotes")]
    public string? OwnerNotes { get; set; }

    public DateTime Created { get; set; }

    [JsonIgnore]
    public CatStatuses Status => Enum.Parse<CatStatuses>(StatusValue);

    [JsonIgnore]
    public string? OwnerRecordId => OwnerRecordIdValue.FirstOrDefault();

    [JsonIgnore]
    public string? OwnerName => OwnerNameValue.FirstOrDefault();

    [JsonIgnore]
    public string OwnerTelegram => OwnerTelegramValue.FirstOrDefault();

    [JsonIgnore]
    public long OwnerTelegramChatId => OwnerTelegramChatIdValue.FirstOrDefault();

    [JsonIgnore]
    public UserRoles? OwnerRole => OwnerRoleValue.FirstOrDefault() != null ?
        Enum.Parse<UserRoles>(OwnerRoleValue.FirstOrDefault()!) :
        null;

    [JsonIgnore]
    public string? AccountPaymentRecordId => AccountPaymentRecordIdValue.FirstOrDefault();

    [JsonIgnore]
    public PaymentOptionTypes? AccountPaymentType => Enum.TryParse<PaymentOptionTypes>(AccountPaymentTypeValue.FirstOrDefault(), out var paymentType) ? paymentType : null;

    [JsonIgnore]
    public CatPaymentStatuses? AccountPaymentStatus => Enum.TryParse<CatPaymentStatuses>(
                                                                AccountPaymentStatusValue
                                                                    .FirstOrDefault(x =>
                                                                        !string.IsNullOrEmpty(x)
                                                                        && x == CatPaymentStatuses.Confirmed.ToString()), out var paymentStatus) ? paymentStatus : null;

    [JsonIgnore]
    public bool OwnerIsVolunteer => OwnerIsVolunteerValue.FirstOrDefault() ?? false;
}
