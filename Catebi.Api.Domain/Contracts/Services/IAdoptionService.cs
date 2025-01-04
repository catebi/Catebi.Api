using AirtableApiClient;

namespace Catebi.Api.Domain.Contracts.Services;

public interface IAdoptionService
{
    Task<List<AirtableRecord>> GetUserRecords();
    Task<bool> ConfirmUser(string recordId);
}
