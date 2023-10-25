using System;
using System.Collections.Generic;

namespace Catebi.Map.Data.Db.Entities;

/// <summary>
/// Словарь: пол
/// </summary>
public partial class CatSex
{
    /// <summary>
    /// Id пола
    /// </summary>
    public int CatSexId { get; set; }

    /// <summary>
    /// Пол: название (м/ж)
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// Id цвета
    /// </summary>
    public int? ColorId { get; set; }

    public virtual ICollection<Cat> Cat { get; set; } = new List<Cat>();

    public virtual Color? Color { get; set; }
}
