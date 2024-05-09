using System;
using System.Collections.Generic;

namespace Catebi.Api.Data.Db.Entities;

/// <summary>
/// Таблица групп слов
/// </summary>
public partial class Group
{
    /// <summary>
    /// ID группы
    /// </summary>
    public int GroupId { get; set; }

    /// <summary>
    /// Название группы
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// Признак актуальности группы
    /// </summary>
    public bool IsActual { get; set; }

    public virtual ICollection<GroupExcludedKeyword> GroupExcludedKeyword { get; set; } = new List<GroupExcludedKeyword>();

    public virtual ICollection<GroupIncludedKeyword> GroupIncludedKeyword { get; set; } = new List<GroupIncludedKeyword>();

    public virtual ICollection<Keyword> Keyword { get; set; } = new List<Keyword>();
}
