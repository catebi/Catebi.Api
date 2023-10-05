using System;
using System.Collections.Generic;

namespace Catebi.Map.WebApi.Db.Catebi.Entities;

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

    public virtual ICollection<Cat2catTag> Cat2catTag { get; set; } = new List<Cat2catTag>();

    public virtual Color? Color { get; set; }
}
