using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;

using Notion.Client;

namespace Catebi.Map.Domain.Services;

public class NotionApiService : INotionApiService
{
    private readonly IMemoryCache _cache;
    private readonly INotionClient _client;
    private readonly CatebiContext _context;
    private readonly NotionApiSettings _notionSettings;
    private readonly ILogger<NotionApiService> _logger;
    private List<Data.Db.Entities.Color> _colors;
    private List<CatSex> _sexes;
    private List<CatCollar> _collars;
    private List<CatTag> _catTags;
    private List<Volunteer> _volunteers;

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
        await SyncDictionaries();
        await InitDictionaries();
        // Optionally, you could cache this key list as well.
        _cache.TryGetValue("CachedCatKeys", out List<string> cachedCatKeys);
        cachedCatKeys ??= [];

        var queryParams = new DatabasesQueryParameters();

        var catsResult = GetAllCatsFromCache(cachedCatKeys);
        if (catsResult.Any())
        {
            var lastUpdated = catsResult.Max(x => x.ChangedDate);

            var lastEditedTimeFilter = new TimestampLastEditedTimeFilter(after: lastUpdated);
            queryParams = new DatabasesQueryParameters { Filter = lastEditedTimeFilter, PageSize = 10 };
            var updatedOrNewCats = await GetCatsFromNotion(queryParams);

            foreach (var cat in updatedOrNewCats)
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

    private async Task InitDictionaries()
    {
        _colors = await _context.Color.ToListAsync();
        _sexes = await _context.CatSex.Include(x => x.Color).ToListAsync();
        _collars = await _context.CatCollar.Include(x => x.Color).ToListAsync();
        _catTags = await _context.CatTag.Include(x => x.Color).ToListAsync();
        _volunteers = await _context.Volunteer.ToListAsync();
    }

    private async Task SyncDictionaries()
    {
        var catsDb = await _client.Databases.RetrieveAsync(_notionSettings.DatabaseIds[NotionDb.Cats]);

        foreach(var prop in catsDb.Properties.Where(x => new [] { "collar", "tags", "room" }.Contains(x.Value.Name)))
        {
            switch (prop.Value.Name)
            {
                case "collar":
                    var collarValues =
                        ((SelectProperty)prop.Value)
                            .Select
                            .Options
                            .Select(x => new {x.Name, x.Color})
                            .ToList();

                    foreach(var val in collarValues)
                    {
                        var entity = await _context.CatCollar.SingleOrDefaultAsync(x => x.Name == val.Name);
                        if (entity == null)
                        {
                            var colorString = val.Color.ToString();
                            Enum.TryParse(colorString, out Colors colorValue);
                            await _context.CatCollar.AddAsync(new CatCollar
                            {
                                Name = val.Name,
                                ColorId = (int)colorValue,
                            });
                        }
                    }

                    break;

                case "tags":
                   var tagValues =
                        ((MultiSelectProperty)prop.Value)
                            .MultiSelect
                            .Options
                            .Select(x => new {x.Name, x.Color})
                            .ToList();

                    foreach(var val in tagValues)
                    {
                        var entity = await _context.CatTag.SingleOrDefaultAsync(x => x.Name == val.Name);
                        if (entity == null)
                        {
                            var colorString = val.Color.ToString();
                            Enum.TryParse(colorString, out Colors colorValue);
                            await _context.CatTag.AddAsync(new CatTag
                            {
                                Name = val.Name,
                                ColorId = (int)colorValue,
                            });
                        }
                    }

                    break;

                case "room":
                    var roomValues =
                        ((SelectProperty)prop.Value)
                            .Select
                            .Options
                            .Select(x => new {x.Name, x.Color})
                            .ToList();

                    foreach(var val in roomValues)
                    {
                        var entity = await _context.CatHouseSpace.SingleOrDefaultAsync(x => x.Name == val.Name);
                        if (entity == null)
                        {
                            var colorString = val.Color.ToString();
                            Enum.TryParse(colorString, out Colors colorValue);
                            await _context.CatHouseSpace.AddAsync(new CatHouseSpace
                            {
                                Name = val.Name,
                                ColorId = (int)colorValue,
                            });
                        }
                    }

                    break;
            }

            await _context.SaveChangesAsync();
        }
    }

    private async Task SaveCatsToDb(List<CatDto> catsResult)
    {
        var cats = catsResult
        .OrderBy(x => int.Parse(x.NotionCatId.Replace("CAT-", "")))
        .Select(x => new Cat
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
            InDate = x.InDate,
            OutDate = x.OutDate,
            NeuteredDate = x.NeuteredDate,
            Comment = x.Comment,
            CatSexId = x.Sex.Id,
            CatCollarId = x.Collar?.Id,
            CatHouseSpaceId = x.Space?.Id,
            CatCatTag = x.Tags.Select(t => new CatCatTag
            {
                CatTagId = t.Id,
            }).ToList(),
            CreatedDate = x.CreatedDate.ToUniversalTime(),
            ChangedDate = x.ChangedDate.ToUniversalTime()
        });

        await _context.Cat.AddRangeAsync(cats);
        await _context.SaveChangesAsync();
    }

    private async Task<List<CatDto>> GetCatsFromNotion(DatabasesQueryParameters queryParams)
    {
        var catsResponse = await _client.Databases.QueryAsync(_notionSettings.DatabaseIds[NotionDb.Cats], queryParams);
        var catsResult = catsResponse.Results.Select(GetCatDto)
                                             .ToList();

        while (catsResponse.HasMore)
        {
            queryParams.StartCursor = catsResponse.NextCursor;
            catsResponse = await _client.Databases.QueryAsync(_notionSettings.DatabaseIds[NotionDb.Cats], queryParams);
            catsResult.AddRange(catsResponse.Results.Select(GetCatDto));
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

    private CatDto GetCatDto(Page x)
    {
        var properties = x.Properties;
        var idProperty = ((UniqueIdPropertyValue)properties["id"]).UniqueId;
        var sexValue = ((SelectPropertyValue)x.Properties["sex"]).Select?.Name ?? "ж";
        var catSex = _sexes.Single(x => x.Name.ToLower() == sexValue.ToLower());

        var collarValue = ((SelectPropertyValue)x.Properties["collar"]).Select?.Name;
        var collar = _collars.SingleOrDefault(x => x.Name.ToLower() == collarValue?.ToLower());

        var tagValues = ((MultiSelectPropertyValue)x.Properties["tags"]).MultiSelect?.Select(x => x.Name).ToList();
        var tags = _catTags.Where(x => tagValues?.Contains(x.Name.ToLower()) ?? false).ToList();

        var catSpace = ((SelectPropertyValue)x.Properties["room"]).Select?.Name;
        var space = _context.CatHouseSpace.SingleOrDefault(x => x.Name == catSpace);

        return new CatDto
        {
            NotionCatId = $"{idProperty.Prefix}-{idProperty.Number}",
            Name = ((TitlePropertyValue)x.Properties["cat\\name"]).Title.FirstOrDefault()?.PlainText,
            GeoLocation = ((RichTextPropertyValue)x.Properties["geo_location"]).RichText.FirstOrDefault()?.PlainText,
            Address = ((RichTextPropertyValue)x.Properties["address"]).RichText.FirstOrDefault()?.PlainText,
            NotionPageUrl = x.Url,
            InDate = ((DatePropertyValue)x.Properties["in_date"]).Date?.Start.ToDateOnly(),
            OutDate = ((DatePropertyValue)x.Properties["out_date"]).Date?.Start.ToDateOnly(),
            NeuteredDate = ((DatePropertyValue)x.Properties["Neutered"]).Date?.Start.ToDateOnly(),
            Comment = ((RichTextPropertyValue)x.Properties["comment"]).RichText.FirstOrDefault()?.PlainText,
            Sex = new LookupDto { Id = catSex.CatSexId, Name = catSex.Name, Color = catSex.Color!.HexCode },
            Collar = collar != null ? new LookupDto { Id = collar.CatCollarId, Name = collar.Name, Color = collar.Color!.HexCode } : null,
            Space = space != null ? new LookupDto { Id = space.CatHouseSpaceId, Name = space.Name, Color = space.Color!.HexCode } : null,
            Tags = tags.Select(x => new LookupDto { Id = x.CatTagId, Name = x.Name, Color = x.Color!.HexCode }).ToList(),
            //ResponsibleVolunteer = ((PeoplePropertyValue)x.Properties["responsible_volunteer"]).People.FirstOrDefault()?.Id,
            Images = ((FilesPropertyValue)x.Properties["Files & media"])
                        .Files
                        .Select(f => new NotionFile { Name = f.Name, Url = ((UploadedFileWithName)f).File.Url, Type = f.Type })
                        .ToList(),
            CreatedDate = x.CreatedTime,
            ChangedDate = x.LastEditedTime
        };
    }
}
