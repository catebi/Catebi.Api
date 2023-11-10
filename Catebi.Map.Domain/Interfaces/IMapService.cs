
namespace Catebi.Map.Domain.Services;

public interface IMapService
{
    Task<IEnumerable<CatDto>> GetCats();
    Task<IEnumerable<CatDtoShort>> GetCatsShort();
    Task<IEnumerable<CatDto>> GetVolunteers();
}