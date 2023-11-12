using System;
using System.Collections.Generic;

namespace Catebi.Api.Data.Db.Entities;

/// <summary>
/// Словарь цветов (для ошейников, отметок и проч)
/// </summary>
public partial class Color
{
    /// <summary>
    /// Id цвета в базе
    /// </summary>
    public int ColorId { get; set; }

    /// <summary>
    /// Название кириллицей
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// Запись в формате &quot;(255, 255, 255)&quot;
    /// </summary>
    public string RgbCode { get; set; } = null!;

    /// <summary>
    /// Запись в формате &quot;#000000&quot;
    /// </summary>
    public string HexCode { get; set; } = null!;

    public virtual ICollection<CatCollar> CatCollar { get; set; } = new List<CatCollar>();

    public virtual ICollection<CatHouseSpace> CatHouseSpace { get; set; } = new List<CatHouseSpace>();

    public virtual ICollection<CatSex> CatSex { get; set; } = new List<CatSex>();

    public virtual ICollection<CatTag> CatTag { get; set; } = new List<CatTag>();
}
