namespace Catebi.Api.Domain.Interfaces;

public interface INotionApiService
{
    Task<bool> SyncDicts();
    Task<bool> SyncVolunteers();
    Task<bool> SyncCats();
}