using Catebi.Api.Domain.Features.AdoptionBot.Enums;

namespace Catebi.Api.Domain.Features.AdoptionBot.Models;

public class CatPayment
{
    public Guid Id { get; set; }
    public string CatRecordId { get; set; } = null!;
    public string ProofUrl { get; set; } = null!;
    public CatPaymentStatuses Status { get; set; }
    public DateTime CreatedAt { get; set; }
}
