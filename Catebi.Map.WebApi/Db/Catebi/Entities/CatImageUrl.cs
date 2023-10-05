using System;
using System.Collections.Generic;

namespace Catebi.Map.WebApi.Db.Catebi.Entities;

/// <summary>
/// Ссылка на картинки для кошек/котов
/// </summary>
public partial class CatImageUrl
{
    /// <summary>
    /// Id
    /// </summary>
    public int CatImageUrlId { get; set; }

    /// <summary>
    /// Id кошки/кота
    /// </summary>
    public int CatId { get; set; }

    /// <summary>
    /// Имя/описание
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Ссылка на картинку
    /// </summary>
    public string Url { get; set; } = null!;

    /// <summary>
    /// Тип картинки
    /// </summary>
    public string Type { get; set; } = null!;

    public virtual Cat Cat { get; set; } = null!;
}
