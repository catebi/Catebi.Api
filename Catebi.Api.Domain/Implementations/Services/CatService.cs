using Microsoft.Extensions.Caching.Memory;

namespace Catebi.Api.Domain.Implementations.Services;

public class CatService(    IUnitOfWork unitOfWork,
                            IMemoryCache cache,
                            ILogger<CatService> logger
                        ) : ICatService
{
    private const string CatsShortKey = "CachedCatShort";
    private const string CachedCatKeysKey = "CachedCatKeys";
    private readonly ICatRepository _catRepo = unitOfWork.CatRepository;
    private readonly ILogger<CatService> _logger = logger;
    private readonly IMemoryCache _cache = cache;

    #region Public
    public async Task<IEnumerable<CatDto>> GetCats()
    {
        var result = new List<CatDto>();
        try
        {
            result = await GetCatsInternal();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error getting cats");
            throw;
        }

        return result;
    }

    public async Task<IEnumerable<CatDtoShort>> GetCatsShort()
    {
        var result = new List<CatDtoShort>();
        try
        {
            result = await GetCatsShortInternal();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error getting cats");
            throw;
        }

        return result;
    }

    #endregion


    #region Private

    private async Task<List<CatDto>> GetCatsInternal()
    {
        // Optionally, you could cache this key list as well.
        _cache.TryGetValue(CachedCatKeysKey, out List<string> cachedCatKeys);
        cachedCatKeys ??= [];

        var catsResult = GetAllCatsFromCache(cachedCatKeys);

        if (!catsResult.Any())
        {
            catsResult = await GetCatsFromDb();
            CacheCats(catsResult);
        }

        return catsResult;
    }

    private async Task<List<CatDtoShort>> GetCatsShortInternal()
    {
        // Optionally, you could cache this key list as well.
        _cache.TryGetValue(CatsShortKey, out List<CatDtoShort> cachedCats);
        cachedCats ??= [];

        if (!cachedCats.Any())
        {
            cachedCats = await GetCatsShortFromDb();
            CacheCatsShort(cachedCats);
        }

        return cachedCats;
    }

    private void CacheCatsShort(List<CatDtoShort> catsResult)
    {
        var cacheOptions = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(60)
        };

        _cache.Set(CatsShortKey, catsResult, cacheOptions);
    }

    private async Task<List<CatDto>> GetCatsFromDb()
    {
        var cats = await _catRepo.GetAsync
        (
            filter: x => x.OutDate.HasValue,
            include: x => x.Include(y => y.CatSex)
                            .ThenInclude(y => y.Color)
                        .Include(y => y.CatCollar)
                            .ThenInclude(y => y.Color)
                        .Include(y => y.CatHouseSpace)
                            .ThenInclude(y => y.Color)
                        .Include(y => y.CatCatTag)
                            .ThenInclude(y => y.CatTag)
                                .ThenInclude(y => y.Color)
                        .Include(y => y.CatImageUrl)
                        .Include(y => y.ResponsibleVolunteer)
        );

        return cats.Select(GetCatDto).ToList();
    }

    private async Task<List<CatDtoShort>> GetCatsShortFromDb()
    {
        var cats = await _catRepo.GetAsync(filter: x => !string.IsNullOrEmpty(x.GeoLocation));
        return cats.Select(x => new CatDtoShort(x.CatId, x.GeoLocation!))
                   .ToList();
    }

    private CatDto GetCatDto(Cat cat) =>

        new()
        {
            Id = cat.CatId,
            NotionCatId = cat.NotionCatId,
            Name = cat.Name,
            Address = cat.Address,
            GeoLocation = cat.GeoLocation,
            NotionPageUrl = cat.NotionPageUrl,
            InDate = cat.InDate,
            OutDate = cat.OutDate,
            NeuteredDate = cat.NeuteredDate,
            Comment = cat.Comment,
            CreatedDate = cat.CreatedDate,
            ResponsibleVolunteer = cat.ResponsibleVolunteerId.HasValue ?
            new VolunteerDto
            {
                Id = cat.ResponsibleVolunteer!.VolunteerId,
                Name = cat.ResponsibleVolunteer.Name,
                TelegramAccount = cat.ResponsibleVolunteer.TelegramAccount,
                NotionVolunteerId = cat.ResponsibleVolunteer.NotionVolunteerId
            }
            : null,
            Sex =
                new LookupDto
                {
                    Id = cat.CatSex.CatSexId,
                    Name = cat.CatSex.Name,
                    Color = cat.CatSex.Color!.HexCode
                },
            Space = cat.CatHouseSpaceId.HasValue ?
                new LookupDto
                {
                    Id = cat.CatHouseSpace!.CatHouseSpaceId,
                    Name = cat.CatHouseSpace.Name,
                    Color = cat.CatHouseSpace.Color!.HexCode
                } : null,
            Collar = cat.CatCollarId.HasValue ?
                new LookupDto
                {
                    Id = cat.CatCollar!.CatCollarId,
                    Name = cat.CatCollar.Name,
                    Color = cat.CatCollar.Color!.HexCode
                } : null,
            Images = cat.CatImageUrl.Select(x => new NotionImage { Url = x.Url }).ToList(),
            Tags = cat.CatCatTag.Select(x => new LookupDto
            {
                Id = x.CatTag.CatTagId,
                Name = x.CatTag.Name,
                Color = x.CatTag.Color!.HexCode
            }).ToList()
        };

    private void CacheCats(List<CatDto> cats)
    {
        // Set cache options.
        var cacheOptions = new MemoryCacheEntryOptions
        {
            // Keep in cache for this time, reset time if accessed.
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(60)
        };

        var cachedCatKeys = new List<string>();
        foreach (var cat in cats)
        {
            var key = "cat-" + cat.Id;

            _cache.Remove(key);
            _cache.Set(key, cat, cacheOptions);

            cachedCatKeys.Add(key);
        }

        // Optionally, you could cache this key list as well.
        _cache.Set(CachedCatKeysKey, cachedCatKeys, cacheOptions);
    }

    private List<CatDto> GetAllCatsFromCache(List<string> cachedCatKeys)
    {
        var cats = new List<CatDto>();
        foreach (var key in cachedCatKeys)
        {
            if (_cache.TryGetValue(key, out CatDto? cachedCat))
            {
                cats.Add(cachedCat!);
            }
        }

        return cats;
    }

    #endregion
}
