using System;
using System.Collections.Generic;

namespace Catebi.Map.Data.Db.Entities;

/// <summary>
/// Словарь для связи кошек и тегов
/// </summary>
public partial class CatCatTag
{
    /// <summary>
    /// Id соотношения
    /// </summary>
    public int CatCatTagId { get; set; }

    /// <summary>
    /// Id кошки
    /// </summary>
    public int CatId { get; set; }

    /// <summary>
    /// Id тега
    /// </summary>
    public int CatTagId { get; set; }

    public virtual Cat Cat { get; set; } = null!;

    public virtual CatTag CatTag { get; set; } = null!;
}
