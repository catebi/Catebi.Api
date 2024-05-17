using System;
using System.Collections.Generic;

namespace Catebi.Api.Data.Db.Entities;

/// <summary>
/// Таблица для хранения задач в чате Catebi
/// </summary>
public partial class WorkTask
{
    /// <summary>
    /// ID записи
    /// </summary>
    public int WorkTaskId { get; set; }

    /// <summary>
    /// ID топика, в котором создана задача
    /// </summary>
    public int WorkTopicId { get; set; }

    /// <summary>
    /// описание задачи
    /// </summary>
    public string Description { get; set; } = null!;

    /// <summary>
    /// ID статуса задачи
    /// </summary>
    public int StatusId { get; set; }

    /// <summary>
    /// Дата создания
    /// </summary>
    public DateTime? CreatedDate { get; set; }

    /// <summary>
    /// Дата изменения
    /// </summary>
    public DateTime? ChangedDate { get; set; }

    /// <summary>
    /// ID волонтёра-автора задачи
    /// </summary>
    public int CreatedById { get; set; }

    /// <summary>
    /// ID волонтёра, изменившего задачу
    /// </summary>
    public int ChangedById { get; set; }

    public virtual Volunteer ChangedBy { get; set; } = null!;

    public virtual Volunteer CreatedBy { get; set; } = null!;

    public virtual WorkTaskStatus Status { get; set; } = null!;

    public virtual ICollection<WorkTaskReminder> WorkTaskReminder { get; set; } = new List<WorkTaskReminder>();

    public virtual ICollection<WorkTaskResponsible> WorkTaskResponsible { get; set; } = new List<WorkTaskResponsible>();

    public virtual WorkTopic WorkTopic { get; set; } = null!;
}
