using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Notion.Client;

namespace Catebi.Map.WebApi.Services;

public class NotionApiService : INotionApiService
{
    private readonly IMemoryCache _cache;
    private readonly INotionClient _client;
    private readonly CatebiContext _context;
    private readonly NotionApiSettings _notionSettings;
    private readonly ILogger<NotionApiService> _logger;

    public NotionApiService(
        INotionClient client,
        CatebiContext context,
        IOptions<NotionApiSettings> notionSettings,
        ILogger<NotionApiService> logger,
        IMemoryCache memoryCache) // Inject the IMemoryCache
    {
        _client = client;
        _context = context;
        _notionSettings = notionSettings.Value;
        _logger = logger;
        _cache = memoryCache;
    }

    public async Task<List<CatDto>> GetCats()
    {
        // Optionally, you could cache this key list as well.
        _cache.TryGetValue("CachedCatKeys", out List<string> cachedCatKeys);
        cachedCatKeys = cachedCatKeys ?? new List<string>();

        var queryParams = new DatabasesQueryParameters();

        var catsResult = GetAllCatsFromCache(cachedCatKeys);
        if (catsResult.Any())
        {
            var lastUpdated = catsResult.Max(x => x.LastEditedTime);

            var lastEditedTimeFilter = new TimestampLastEditedTimeFilter(after: lastUpdated);
            queryParams = new DatabasesQueryParameters { Filter = lastEditedTimeFilter, PageSize = 10 };
            var updatedOrNewCats = await GetCatsFromNotion(queryParams);

            foreach(var cat in updatedOrNewCats)
            {
                var cachedCat = catsResult.FirstOrDefault(x => x.NotionCatId == cat.NotionCatId);
                if (cachedCat != null)
                {
                    catsResult.Remove(cachedCat);
                }
                catsResult.Add(cat);

                // Update the cache
                var key = cat.NotionCatId;
                _cache.Remove(key);
                _cache.Set(key, cat);

                cachedCatKeys.Remove(key);
                cachedCatKeys.Add(key);
            }

            return catsResult;
        }

        catsResult = await GetCatsFromNotion(queryParams);

        // Save individual cats in the cache
        CacheCats(catsResult);

        await SaveCatsToDb(catsResult);

        return catsResult;
    }

    private async Task SaveCatsToDb(List<CatDto> catsResult)
    {
        var cats = catsResult.Select(x => new Cat
        {
            NotionCatId = x.NotionCatId,
            Name = x.Name,
            GeoLocation = x.GeoLocation,
            Address = x.Address,
            NotionPageUrl = x.NotionPageUrl,
            CatImageUrl = x.Images.Select(i =>
                            new CatImageUrl
                            {
                                Name = i.Name,
                                Url = i.Url,
                                Type = i.Type
                            }).ToList(),
            CreatedTime = x.CreatedTime.ToUniversalTime(),
            LastEditedTime = x.LastEditedTime.ToUniversalTime()
        });

        _context.Cat.AddRange(cats);
        await _context.SaveChangesAsync();
    }

    private async Task<List<CatDto>> GetCatsFromNotion(DatabasesQueryParameters queryParams)
    {
        var catsResponse = await _client.Databases.QueryAsync(_notionSettings.DatabaseIds[NotionDb.Cats], queryParams);

        var catsResult = catsResponse.Results.Select(GetCatDto())
                                             .ToList();

        while (catsResponse.HasMore)
        {
            queryParams.StartCursor = catsResponse.NextCursor;
            catsResponse = await _client.Databases.QueryAsync(_notionSettings.DatabaseIds[NotionDb.Cats], queryParams);
            catsResult.AddRange(catsResponse.Results.Select(GetCatDto()));
        }

        return catsResult;
    }

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
            var key = cat.NotionCatId;

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

    private static Func<Page, CatDto> GetCatDto()
    {
        return x =>
        {
            var properties = x.Properties;
            var idProperty = ((UniqueIdPropertyValue)properties["id"]).UniqueId;
            return new CatDto
            {
                NotionCatId = $"{idProperty.Prefix}-{idProperty.Number}",
                Name = ((TitlePropertyValue)x.Properties["cat\\name"]).Title.FirstOrDefault()?.PlainText,
                GeoLocation = ((RichTextPropertyValue)x.Properties["geo_location"]).RichText.FirstOrDefault()?.PlainText,
                Address = ((RichTextPropertyValue)x.Properties["address"]).RichText.FirstOrDefault()?.PlainText,
                NotionPageUrl = x.Url,
                Images = ((FilesPropertyValue)x.Properties["Files & media"])
                            .Files
                            .Select(f => new NotionFile { Name = f.Name, Url = ((UploadedFileWithName)f).File.Url, Type = f.Type })
                            .ToList(),
                CreatedTime = x.CreatedTime,
                LastEditedTime = x.LastEditedTime
            };
        };
    }
}
