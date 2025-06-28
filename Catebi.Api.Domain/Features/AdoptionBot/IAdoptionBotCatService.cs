namespace Catebi.Api.Domain.Features.AdoptionBot;

public interface IAdoptionBotCatService
{
    /// <summary>
    /// Add a new cat record
    /// </summary>
    Task<CatDto> AddCat(CatDto cat);

    /// <summary>
    /// Get all cats for a user
    /// </summary>
    Task<IEnumerable<CatDto>> GetCatsByUserId(string userId);

    /// <summary>
    /// Get a cat by record ID
    /// </summary>
    Task<CatDto?> GetCatById(string recordId);

    /// <summary>
    /// Update a cat record
    /// </summary>
    Task<CatDto> UpdateCat(CatDto cat);

    /// <summary>
    /// Get payments for a specific cat
    /// </summary>
    Task<IEnumerable<CatPaymentDto>> GetCatPayments(string catId);

    /// <summary>
    /// Add a photo to a cat's record and notify the owner.
    /// </summary>
    Task<bool> AddCatPhoto(string catRecordId, string photoUrl);

    /// <summary>
    /// Add a payment confirmation for a cat and notify the owner.
    /// </summary>
    Task<CatPaymentDto> AddCatPayment(string catRecordId, string imageUrl);

    /// <summary>
    /// Register a cat to an event.
    /// </summary>
    Task<bool> RegisterCatToEvent(string catRecordId, string eventRecordId);
}
