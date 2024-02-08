using System;
using System.Collections.Generic;

namespace Catebi.Api.Data.Db.Entities;

/// <summary>
/// Единицы измерения времени
/// </summary>
public partial class TimeUnit
{
    /// <summary>
    /// ID единицы измерения
    /// </summary>
    public int TimeUnitId { get; set; }

    /// <summary>
    /// Наименование единицы измерения
    /// </summary>
    public string Name { get; set; } = null!;

    public virtual ICollection<Prescription> Prescription { get; set; } = new List<Prescription>();
}
