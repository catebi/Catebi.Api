using System;
using System.Collections.Generic;

namespace Catebi.Map.WebApi.Db.Catebi.Entities;

/// <summary>
/// Словарь для связи кошек и тегов
/// </summary>
public partial class Cat2catTag
{
    /// <summary>
    /// Id соотношения
    /// </summary>
    public int Cat2catTagId { get; set; }

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
