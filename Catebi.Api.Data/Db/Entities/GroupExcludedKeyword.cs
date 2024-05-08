using System;
using System.Collections.Generic;

namespace Catebi.Api.Data.Db.Entities;

/// <summary>
/// Таблица исключенных ключевых слов для группы
/// </summary>
public partial class GroupExcludedKeyword
{
    /// <summary>
    /// ID исключенного ключевого слова
    /// </summary>
    public int ExcludedKeywordId { get; set; }

    /// <summary>
    /// ID группы
    /// </summary>
    public int GroupId { get; set; }

    /// <summary>
    /// Исключенное ключевое слово
    /// </summary>
    public string Keyword { get; set; } = null!;

    public virtual Group Group { get; set; } = null!;
}
