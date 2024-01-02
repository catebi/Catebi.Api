namespace Catebi.Api.Domain.Contracts.Services;

public interface ICatService
{
    Task<IEnumerable<CatDto>> GetCats();
    Task<IEnumerable<CatDtoShort>> GetCatsShort();
}