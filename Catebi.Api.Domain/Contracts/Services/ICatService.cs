namespace Catebi.Api.Domain.Contracts.Services;

public interface ICatService
{
    Task<IEnumerable<MapCatDto>> GetCats();
    Task<IEnumerable<CatDtoShort>> GetCatsShort();
}