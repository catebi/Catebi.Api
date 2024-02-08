using System;
using System.Collections.Generic;

namespace Catebi.Api.Data.Db.Entities;

/// <summary>
/// Словарь: теги кошек
/// </summary>
public partial class CatTag
{
    /// <summary>
    /// Id тега
    /// </summary>
    public int CatTagId { get; set; }

    /// <summary>
    /// Текст тега (&quot;медуход&quot;, &quot;аборт&quot; итп)
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// Id цвета
    /// </summary>
    public int? ColorId { get; set; }

    public virtual ICollection<CatCatTag> CatCatTag { get; set; } = new List<CatCatTag>();

    public virtual Color? Color { get; set; }
}
