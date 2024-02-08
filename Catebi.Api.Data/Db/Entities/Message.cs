using System;
using System.Collections.Generic;

namespace Catebi.Api.Data.Db.Entities;

/// <summary>
/// Архив сообщений, прошедших через бот для разметки
/// </summary>
public partial class Message
{
    /// <summary>
    /// ID записи
    /// </summary>
    public int MessageId { get; set; }

    /// <summary>
    /// Исходный текст сообщения
    /// </summary>
    public string OriginalText { get; set; } = null!;

    /// <summary>
    /// Текст сообщения после лемматизации
    /// </summary>
    public string LemmatizedText { get; set; } = null!;

    /// <summary>
    /// Ссылка на чат, откуда бот сообщение взял
    /// </summary>
    public string ChatLink { get; set; } = null!;

    /// <summary>
    /// Принято ли сообщение по текущему набору правил
    /// </summary>
    public bool Accepted { get; set; }
}
