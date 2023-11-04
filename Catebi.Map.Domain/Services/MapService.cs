using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Catebi.Map.Domain.Services;

public class MapService : IMapService
{
    private readonly CatebiContext _context;
    private readonly ILogger<MapService> _logger;
    private readonly IMemoryCache _cache;

    public MapService(CatebiContext context, IMemoryCache cache, ILogger<MapService> logger)
    {
        _context = context;
        _logger = logger;
        _cache = cache;
    }

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

    private async Task<List<CatDto>> GetCatsInternal()
    {
        // Optionally, you could cache this key list as well.
        _cache.TryGetValue("CachedCatKeys", out List<string> cachedCatKeys);
        cachedCatKeys ??= [];

        var catsResult = GetAllCatsFromCache(cachedCatKeys);

        if (!catsResult.Any())
        {
            catsResult = await GetCatsFromDb();
            CacheCats(catsResult);
        }

        return catsResult;
    }

    private async Task<List<CatDto>> GetCatsFromDb()
    {
        var cats = await _context.Cat
                                 .Include(x => x.CatSex)
                                    .ThenInclude(x => x.Color)
                                 .Include(x => x.CatCollar)
                                    .ThenInclude(x => x.Color)
                                 .Include(x => x.CatHouseSpace)
                                    .ThenInclude(x => x.Color)
                                 .Include(x => x.CatCatTag)
                                    .ThenInclude(x => x.CatTag)
                                        .ThenInclude(x => x.Color)
                                 .Include(x => x.CatImageUrl)
                                 .Include(x => x.ResponsibleVolunteer)
                                 .Where(x => x.OutDate.HasValue)
                                 .ToListAsync();

        return cats.Select(GetCatDto).ToList();
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
            Id = cat.ResponsibleVolunteer.VolunteerId,
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


    public Task<IEnumerable<CatDto>> GetVolunteers()
    {
        throw new NotImplementedException();
    }

    #region Private

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
        _cache.Set("CachedCatKeys", cachedCatKeys, cacheOptions);
    }

    public List<CatDto> GetAllCatsFromCache(List<string> cachedCatKeys)
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
