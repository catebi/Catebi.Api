using System;
using System.Collections.Generic;

namespace Catebi.Api.Data.Db.Entities;

/// <summary>
/// Таблица ключевых слов
/// </summary>
public partial class Keyword
{
    /// <summary>
    /// ID ключевого слова
    /// </summary>
    public int KeywordId { get; set; }

    /// <summary>
    /// ID группы, к которой относится ключевое слово
    /// </summary>
    public int GroupId { get; set; }

    /// <summary>
    /// Ключевое слово
    /// </summary>
    public string Keyword1 { get; set; } = null!;

    public virtual Group Group { get; set; } = null!;
}
