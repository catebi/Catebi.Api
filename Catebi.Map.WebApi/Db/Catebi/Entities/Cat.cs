using System;
using System.Collections.Generic;

namespace Catebi.Map.WebApi.Db.Catebi.Entities;

/// <summary>
/// Кошка/кот
/// </summary>
public partial class Cat
{
    /// <summary>
    /// Ид.
    /// </summary>
    public int CatId { get; set; }

    /// <summary>
    /// Ид. в Notion
    /// </summary>
    public string? NotionCatId { get; set; }

    /// <summary>
    /// Имя/описание
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// Адрес
    /// </summary>
    public string? Address { get; set; }

    /// <summary>
    /// Геолокация
    /// </summary>
    public string? GeoLocation { get; set; }

    /// <summary>
    /// Дата создания
    /// </summary>
    public DateTime CreatedTime { get; set; }

    /// <summary>
    /// Дата последнего изменения
    /// </summary>
    public DateTime LastEditedTime { get; set; }

    /// <summary>
    /// Линк на страницу в Notion
    /// </summary>
    public string? NotionPageUrl { get; set; }

    public virtual ICollection<CatImageUrl> CatImageUrl { get; set; } = new List<CatImageUrl>();
}
