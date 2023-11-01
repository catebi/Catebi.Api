namespace Catebi.Map.Domain.Interfaces;

public interface INotionApiService
{
    Task<bool> SyncDicts();
    Task<bool> SyncVolunteers();
    Task<bool> SyncCats();
}