using System;
using System.Collections.Generic;

namespace Catebi.Api.Data.Db.Entities;

/// <summary>
/// Таблица для хранения инфоромации о топиках в чате Catebi
/// </summary>
public partial class WorkTopic
{
    /// <summary>
    /// ID записи
    /// </summary>
    public int WorkTopicId { get; set; }

    /// <summary>
    /// ID топика в tg
    /// </summary>
    public int TelegramThreadId { get; set; }

    /// <summary>
    /// Название топика
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// описание топика
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// топик для напоминаний об активных задачах
    /// </summary>
    public bool IsMain { get; set; }

    /// <summary>
    /// Флаг актуальности
    /// </summary>
    public bool IsActual { get; set; }

    /// <summary>
    /// Дата создания
    /// </summary>
    public DateTime? Created { get; set; }

    /// <summary>
    /// ID волонтёра-автора топика
    /// </summary>
    public int CreatedById { get; set; }

    public virtual ICollection<WorkTask> WorkTask { get; set; } = new List<WorkTask>();
}
