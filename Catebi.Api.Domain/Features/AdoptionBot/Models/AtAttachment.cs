using System.Text.Json.Serialization;

namespace Catebi.Api.Domain.Features.AdoptionBot.Models;

public class AtAttachment
{
    #region At fields

    [JsonPropertyName("id")]
    public string Id { get; set; }
    [JsonPropertyName("url")]
    public string Url { get; set; }
    [JsonPropertyName("fileName")]
    public string FileName { get; set; }

    #endregion
}
