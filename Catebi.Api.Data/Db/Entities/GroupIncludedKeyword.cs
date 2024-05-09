using System;
using System.Collections.Generic;

namespace Catebi.Api.Data.Db.Entities;

/// <summary>
/// Таблица включенных ключевых слов для группы
/// </summary>
public partial class GroupIncludedKeyword
{
    /// <summary>
    /// ID включенного ключевого слова
    /// </summary>
    public int IncludedKeywordId { get; set; }

    /// <summary>
    /// ID группы
    /// </summary>
    public int GroupId { get; set; }

    /// <summary>
    /// Включенное ключевое слово
    /// </summary>
    public string Keyword { get; set; } = null!;

    public virtual Group Group { get; set; } = null!;
}
