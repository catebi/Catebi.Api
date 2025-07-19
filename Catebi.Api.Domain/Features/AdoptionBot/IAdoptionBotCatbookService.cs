using Catebi.Api.Domain.Features.AdoptionBot.ViewModels;

namespace Catebi.Api.Domain.Features.AdoptionBot;

public interface IAdoptionBotCatbookService
{
    /// <summary>
    /// Get catbook information by cat record ID
    /// </summary>
    Task<CatbookInfoDto?> GetCatbookInfo(string catRecordId);

    /// <summary>
    /// Save new catbook information with ToConfirm status
    /// </summary>
    Task<CatbookInfoDto> SaveCatbookInfo(CatbookInfoDto catbookInfo);

    /// <summary>
    /// Update existing catbook information (except status)
    /// </summary>
    Task<CatbookInfoDto> UpdateCatbookInfo(CatbookInfoDto catbookInfo);

    /// <summary>
    /// Confirm catbook information and send new post to the catbook telegram channel
    /// </summary>
    Task<bool> ConfirmCatbookInfo(string catRecordId);

    /// <summary>
    /// Archive catbook information
    /// </summary>
    Task<bool> ArchiveCatbookInfo(string catRecordId);

    /// <summary>
    /// Get all catbook information records with ToConfirm status for admin
    /// </summary>
    Task<IEnumerable<CatbookInfoDto>> GetCatbookInfosToConfirm();
} 