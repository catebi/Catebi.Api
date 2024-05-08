using System;
using System.Collections.Generic;

namespace Catebi.Api.Data.Db.Entities;

/// <summary>
/// Таблица для сбора статисти реакций на сообщения
/// </summary>
public partial class DonationMessageReaction
{
    /// <summary>
    /// ID
    /// </summary>
    public int DonationMessageReactionId { get; set; }

    /// <summary>
    /// ID сообщения (в чате после фильтрации)
    /// </summary>
    public int MessageId { get; set; }

    /// <summary>
    /// Текст сообщения
    /// </summary>
    public string Content { get; set; } = null!;

    /// <summary>
    /// Количество реакций 👍
    /// </summary>
    public int LikeCount { get; set; }

    /// <summary>
    /// Количество реакций 👎
    /// </summary>
    public int DislikeCount { get; set; }
}
