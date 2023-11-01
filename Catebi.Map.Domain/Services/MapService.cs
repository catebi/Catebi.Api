using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Catebi.Map.Domain.Services;

public class MapService : IMapService
{
    private readonly ILogger<MapService> _logger;
    private readonly IMemoryCache _cache;

    public MapService(IMemoryCache cache)
    {
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
        // _cache.TryGetValue("CachedCatKeys", out List<string> cachedCatKeys);
        // cachedCatKeys ??= [];

        // var catsResult = GetAllCatsFromCache(cachedCatKeys);
        // catsResult = await GetCatsFromDb(queryParams);

        // // Save individual cats in the cache
        // CacheCats(catsResult);
        throw new NotImplementedException();
    }

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
