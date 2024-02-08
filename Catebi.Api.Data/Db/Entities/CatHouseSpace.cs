using System;
using System.Collections.Generic;

namespace Catebi.Api.Data.Db.Entities;

/// <summary>
/// Словарь: котоквартира
/// </summary>
public partial class CatHouseSpace
{
    /// <summary>
    /// Id комнаты
    /// </summary>
    public int CatHouseSpaceId { get; set; }

    /// <summary>
    /// Название комнаты
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// Id цвета
    /// </summary>
    public int ColorId { get; set; }

    public virtual ICollection<Cat> Cat { get; set; } = new List<Cat>();

    public virtual Color Color { get; set; } = null!;
}
