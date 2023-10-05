using System;
using System.Collections.Generic;

namespace Catebi.Map.WebApi.Db.Catebi.Entities;

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
    /// Сокращение &quot;Комната1&quot;--&gt;&quot;К1&quot;
    /// </summary>
    public string? ShortName { get; set; }

    /// <summary>
    /// Id цвета
    /// </summary>
    public int ColorId { get; set; }

    public virtual ICollection<Cat> Cat { get; set; } = new List<Cat>();

    public virtual Color Color { get; set; } = null!;
}
