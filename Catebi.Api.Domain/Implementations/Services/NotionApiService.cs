using Microsoft.Extensions.Options;
using Notion.Client;

namespace Catebi.Api.Domain.Implementations.Services;

public class NotionApiService(
    INotionClient client,
    IOptions<NotionApiSettings> notionSettings,
    CatebiContext context,
    ILogger<NotionApiService> logger) : INotionApiService
{
    private readonly INotionClient _client = client;
    private readonly CatebiContext _context = context;
    private readonly NotionApiSettings _notionSettings = notionSettings.Value;
    private readonly ILogger<NotionApiService> _logger = logger;

    private List<CatSex> _sexes;
    private List<CatCollar> _collars;
    private List<CatTag> _catTags;
    private List<Volunteer> _volunteers;
    private List<VolunteerDto> _volunteerDtos;
    private List<CatDto> _catDtos;
    private Dictionary<string, string> _notionVolunteerPageDict = [];

    #region Public

    public async Task<bool> SyncDicts()
    {
        try
        {
            await SyncDictsInternal();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error syncing dictionaries");
            return false;
        }

        return true;
    }

    public async Task<bool> SyncVolunteers()
    {
        try
        {
            await SyncVolunteersInternal();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error syncing volunteers");
            return false;
        }

        return true;
    }

    public async Task<bool> SyncCats()
    {
        try
        {
            await InitDictionaries();
            await SyncCatsInternal();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error syncing cats");
            return false;
        }

        return true;
    }

#endregion

#region Private

    private async Task SyncDictsInternal()
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

    private async Task SyncVolunteersInternal()
    {
        await GetVolunteersFromNotion();
        await SaveVolunteersToDb();
    }

    private async Task SyncCatsInternal()
    {
        await GetCatsFromNotion();
        await SaveCatsToDb();

        // Save all cats to cache
        // CacheCats(catsResult);
    }

    private async Task InitDictionaries()
    {
        _sexes = await _context.CatSex.Include(x => x.Color).ToListAsync();
        _collars = await _context.CatCollar.Include(x => x.Color).ToListAsync();
        _catTags = await _context.CatTag.Include(x => x.Color).ToListAsync();
        _volunteers = await _context.Volunteer.ToListAsync();
    }

    private async Task GetVolunteersFromNotion()
    {
        var queryParams = new DatabasesQueryParameters();
        var volunteerResponse = await _client.Databases.QueryAsync(_notionSettings.DatabaseIds[NotionDb.Volunteers], queryParams);
        var volunteerResult = volunteerResponse.Results.Select(GetVolunteerDto)
                                             .ToList();

        while (volunteerResponse.HasMore)
        {
            queryParams.StartCursor = volunteerResponse.NextCursor;
            volunteerResponse = await _client.Databases.QueryAsync(_notionSettings.DatabaseIds[NotionDb.Volunteers], queryParams);
            volunteerResult.AddRange(volunteerResponse.Results.Select(GetVolunteerDto));
        }

        _volunteerDtos = volunteerResult;
    }

    private async Task GetCatsFromNotion()
    {
        var queryParams = new DatabasesQueryParameters();
        var catsResponse = await _client.Databases.QueryAsync(_notionSettings.DatabaseIds[NotionDb.Cats], queryParams);
        var catsResult = catsResponse.Results.Select(GetCatDto)
                                             .ToList();

        while (catsResponse.HasMore)
        {
            queryParams.StartCursor = catsResponse.NextCursor;
            catsResponse = await _client.Databases.QueryAsync(_notionSettings.DatabaseIds[NotionDb.Cats], queryParams);
            catsResult.AddRange(catsResponse.Results.Select(GetCatDto));
        }

        foreach(var notionVolunteerPage in _notionVolunteerPageDict)
        {
            var cat = catsResult.Single(x => x.NotionCatId == notionVolunteerPage.Key);
            var volunteerPage = await _client.Pages.RetrieveAsync(notionVolunteerPage.Value);
            var volunteerId =  ((UniqueIdPropertyValue)volunteerPage.Properties["id"]).UniqueId;
            var volunteer = _volunteers.SingleOrDefault(x => x.NotionVolunteerId == volunteerId.Prefix + "-" + volunteerId.Number);
            if (volunteer != null)
            {
                cat.ResponsibleVolunteer = new VolunteerDto
                {
                    Id = volunteer.VolunteerId,
                    Name = volunteer.Name,
                    TelegramAccount = volunteer.TelegramAccount,
                    Address = volunteer.Address,
                    GeoLocation = volunteer.GeoLocation,
                    NotionVolunteerId = volunteer.NotionVolunteerId
                };
            }
        }

        _catDtos = catsResult;
    }

    private VolunteerDto GetVolunteerDto(Page x)
    {
        var properties = x.Properties;
        var idProperty = ((UniqueIdPropertyValue)properties["id"]).UniqueId;
        var notionUserMention = ((RichTextPropertyValue)x.Properties["notion_user"]).RichText;
        return new VolunteerDto
        {
            NotionVolunteerId = $"{idProperty.Prefix}-{idProperty.Number}",
            Name = ((TitlePropertyValue)x.Properties["name"]).Title.FirstOrDefault()?.PlainText,
            GeoLocation = ((RichTextPropertyValue)x.Properties["geo_location"]).RichText.FirstOrDefault()?.PlainText,
            Address = ((RichTextPropertyValue)x.Properties["address"]).RichText.FirstOrDefault()?.PlainText,
            TelegramAccount = ((RichTextPropertyValue)x.Properties["telegram_account"]).RichText.FirstOrDefault()?.PlainText,
            NotionUser = notionUserMention.Any() ? notionUserMention.Where(x => x is RichTextMention)
                                                                    .Select(x => x as RichTextMention)
                                                                    .FirstOrDefault().Mention.User.Id
                                                 : null,
        };
    }

    private CatDto GetCatDto(Page x)
    {
        var properties = x.Properties;
        var idProperty = ((UniqueIdPropertyValue)properties["id"]).UniqueId;
        var notionCatId = $"{idProperty.Prefix}-{idProperty.Number}";
        var sexValue = ((SelectPropertyValue)x.Properties["sex"]).Select?.Name ?? "ж";
        var catSex = _sexes.Single(x => x.Name.ToLower() == sexValue.ToLower());

        var collarValue = ((SelectPropertyValue)x.Properties["collar"]).Select?.Name;
        var collar = _collars.SingleOrDefault(x => x.Name.ToLower() == collarValue?.ToLower());

        var tagValues = ((MultiSelectPropertyValue)x.Properties["tags"]).MultiSelect?.Select(x => x.Name).ToList();
        var tags = _catTags.Where(x => tagValues?.Contains(x.Name.ToLower()) ?? false).ToList();

        var catSpace = ((SelectPropertyValue)x.Properties["room"]).Select?.Name;
        var space = _context.CatHouseSpace.SingleOrDefault(x => x.Name == catSpace);

        var notionUserId = ((RelationPropertyValue)x.Properties["volunteer"]).Relation.FirstOrDefault()?.Id;
        if (!string.IsNullOrWhiteSpace(notionUserId))
        {
            _notionVolunteerPageDict.Add(notionCatId, notionUserId);
        }

        return new CatDto
        {
            NotionCatId = notionCatId,
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
            Images = ((FilesPropertyValue)x.Properties["Files & media"])
                        .Files
                        .Select(f => new NotionImage { Name = f.Name, Url = ((UploadedFileWithName)f).File.Url, Type = f.Type })
                        .ToList(),
            CreatedDate = x.CreatedTime,
            ChangedDate = x.LastEditedTime
        };
    }

    private async Task SaveVolunteersToDb()
    {
        var volunteers = _volunteerDtos
        .OrderBy(x => int.Parse(x.NotionVolunteerId.Replace("VOLUNTEER-", "")))
        .Select(x => new Volunteer
        {
            NotionVolunteerId = x.NotionVolunteerId,
            Name = x.Name,
            GeoLocation = x.GeoLocation,
            Address = x.Address,
            TelegramAccount = x.TelegramAccount,
            NotionUserId = x.NotionUser
            // CreatedDate = x.CreatedDate.ToUniversalTime(),
            // ChangedDate = x.ChangedDate.ToUniversalTime()
        });

        await _context.Volunteer.AddRangeAsync(volunteers);
        await _context.SaveChangesAsync();
    }

    private async Task SaveCatsToDb()
    {
        var cats = _catDtos
        .Where(x => !string.IsNullOrWhiteSpace(x.Name))
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
            ResponsibleVolunteerId = x.ResponsibleVolunteer?.Id,
            CreatedDate = x.CreatedDate.ToUniversalTime(),
            ChangedDate = x.ChangedDate.ToUniversalTime()
        });

        await _context.Cat.AddRangeAsync(cats);
        await _context.SaveChangesAsync();
    }

#endregion

}
