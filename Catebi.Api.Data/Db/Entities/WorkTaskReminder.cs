using System;
using System.Collections.Generic;

namespace Catebi.Api.Data.Db.Entities;

/// <summary>
/// Информация для оповещений по задачам
/// </summary>
public partial class WorkTaskReminder
{
    /// <summary>
    /// ID записи
    /// </summary>
    public int WorkTaskReminderId { get; set; }

    /// <summary>
    /// ID задачи
    /// </summary>
    public int WorkTaskId { get; set; }

    /// <summary>
    /// дата оповещения
    /// </summary>
    public DateTime ReminderDate { get; set; }

    /// <summary>
    /// дата создания
    /// </summary>
    public DateTime Created { get; set; }

    /// <summary>
    /// ID волонтёра-создателя задачи
    /// </summary>
    public int CreatedById { get; set; }

    public virtual Volunteer CreatedBy { get; set; } = null!;

    public virtual WorkTask WorkTask { get; set; } = null!;
}
