using Catebi.Api.Domain.Features.AdoptionBot.Enums;
using System.Text.Json.Serialization;

namespace Catebi.Api.Domain.Features.AdoptionBot.Models;

public class AtPaymentOption
{
    public int PaymentOptionId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int Price { get; set; }

    [JsonPropertyName("Type")]
    public string TypeValue { get; set; }

    [JsonIgnore]
    public PaymentOptionTypes Type => Enum.Parse<PaymentOptionTypes>(TypeValue);
}
