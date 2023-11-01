using Notion.Client;

namespace Catebi.Map.Domain.Interfaces;

public interface INotionApiService
{
    Task<List<CatDto>> GetCats();
}