
namespace Catebi.Map.Domain.Services;

public interface IMapService
{
    Task<IEnumerable<CatDto>> GetCats();
    Task<IEnumerable<CatDto>> GetVolunteers();
}