using System.Text.Json.Serialization;
using Catebi.Api.Domain.Features.AdoptionBot.Enums;

namespace Catebi.Api.Domain.Features.AdoptionBot.Models;

public class AtMessage
{
    public string? RecordId { get; set; }
    public int MessageId { get; set; }
    public string Content { get; set; }
    public string? Log { get; set; }
    
    [JsonPropertyName("Admin")]
    public string[] AdminValue { get; set; } = [];
    
    [JsonPropertyName("Status")]
    public string StatusValue { get; set; } = MessageStatuses.SuccessfullySent.ToString();
    
    public DateTime? Created { get; set; }

    [JsonIgnore]
    public string? AdminRecordId => AdminValue.FirstOrDefault();
    
    [JsonIgnore]
    public MessageStatuses Status => Enum.Parse<MessageStatuses>(StatusValue);
} 