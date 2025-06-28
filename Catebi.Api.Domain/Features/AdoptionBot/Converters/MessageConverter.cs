namespace Catebi.Api.Domain.Features.AdoptionBot.Converters;

public static class MessageConverter
{
    public static MessageDto ToDto(AtMessage message) => new()
    {
        RecordId = message.RecordId,
        MessageId = message.MessageId,
        Content = message.Content,
        AdminRecordId = message.AdminRecordId,
        Created = message.Created?.ToString("yyyy-MM-dd HH:mm:ss")
    };

    public static AtMessage ToAtMessage(MessageDto dto) => new()
    {
        RecordId = dto.RecordId,
        MessageId = dto.MessageId ?? 0,
        Content = dto.Content,
        AdminValue = string.IsNullOrEmpty(dto.AdminRecordId) ? [] : [dto.AdminRecordId],
        Created = string.IsNullOrEmpty(dto.Created) ? null : DateTime.Parse(dto.Created)
    };
}
