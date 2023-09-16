using Notion.Client;

namespace Catebi.Map.WebApi.Interfaces;

public interface INotionApiService
{
    Task<List<CatDto>> GetCats();
}