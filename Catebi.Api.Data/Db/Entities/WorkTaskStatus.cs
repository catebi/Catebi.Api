using System;
using System.Collections.Generic;

namespace Catebi.Api.Data.Db.Entities;

/// <summary>
/// Справочная таблица, содержащая возможные статусы задачек
/// </summary>
public partial class WorkTaskStatus
{
    /// <summary>
    /// ID записи
    /// </summary>
    public int WorkTaskStatusId { get; set; }

    /// <summary>
    /// Код статуса
    /// </summary>
    public int Code { get; set; }

    /// <summary>
    /// Статусы задачек
    /// </summary>
    public string Name { get; set; } = null!;

    public virtual ICollection<WorkTask> WorkTask { get; set; } = new List<WorkTask>();
}
