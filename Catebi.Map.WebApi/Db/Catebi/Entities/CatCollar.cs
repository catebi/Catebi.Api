using System;
using System.Collections.Generic;

namespace Catebi.Map.WebApi.Db.Catebi.Entities;

/// <summary>
/// Словарь: ошейник
/// </summary>
public partial class CatCollar
{
    /// <summary>
    /// Id ошейника
    /// </summary>
    public int CatCollarId { get; set; }

    /// <summary>
    /// Название ошейника (обычно по его цвету)
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// Id цвета
    /// </summary>
    public int ColorId { get; set; }

    public virtual ICollection<Cat> Cat { get; set; } = new List<Cat>();

    public virtual Color Color { get; set; } = null!;
}
