namespace Catebi.Api.Domain.Contracts.Services;

public interface INotionApiService
{
    Task<bool> SyncDicts();
    Task<bool> SyncVolunteers();
    Task<bool> SyncCats();
}