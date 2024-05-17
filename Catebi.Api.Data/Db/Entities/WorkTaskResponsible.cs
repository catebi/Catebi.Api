using System;
using System.Collections.Generic;

namespace Catebi.Api.Data.Db.Entities;

/// <summary>
/// Информация о волонтёре, ответственном за задачу
/// </summary>
public partial class WorkTaskResponsible
{
    /// <summary>
    /// ID записи
    /// </summary>
    public int WorkTaskResponsibleId { get; set; }

    /// <summary>
    /// ID задачи
    /// </summary>
    public int WorkTaskId { get; set; }

    /// <summary>
    /// ID волонтёра, ответственного за задачу
    /// </summary>
    public int VolunteerId { get; set; }

    public virtual Volunteer Volunteer { get; set; } = null!;

    public virtual WorkTask WorkTask { get; set; } = null!;
}
