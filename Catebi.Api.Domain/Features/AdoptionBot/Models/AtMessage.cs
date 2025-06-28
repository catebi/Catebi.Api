using System.Text.Json.Serialization;

namespace Catebi.Api.Domain.Features.AdoptionBot.Models;

public class AtMessage
{
    public string? RecordId { get; set; }
    public int MessageId { get; set; }
    public string Content { get; set; }
    
    [JsonPropertyName("Admin")]
    public string[] AdminValue { get; set; } = [];
    
    public DateTime? Created { get; set; }

    [JsonIgnore]
    public string? AdminRecordId => AdminValue.FirstOrDefault();
} 