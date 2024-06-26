﻿using System;
using System.Collections.Generic;

namespace Catebi.Api.Data.Db.Entities;

/// <summary>
/// Справочник волонтёров
/// </summary>
public partial class Volunteer
{
    /// <summary>
    /// Id волонтёра
    /// </summary>
    public int VolunteerId { get; set; }

    /// <summary>
    /// Id записи о волонтёре в Notion
    /// </summary>
    public string? NotionVolunteerId { get; set; }

    /// <summary>
    /// Имя/ник волонтёра
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// Id аккаунта волонтёра в Notion
    /// </summary>
    public string? NotionUserId { get; set; }

    /// <summary>
    /// Telegram username волонтёра
    /// </summary>
    public string? TelegramAccount { get; set; }

    /// <summary>
    /// Физический (обычно неполный) адрес проживания волонтёра
    /// </summary>
    public string? Address { get; set; }

    /// <summary>
    /// Координаты в пригодном для экспорта формате
    /// </summary>
    public string? GeoLocation { get; set; }

    /// <summary>
    /// Дата создания
    /// </summary>
    public DateTime CreatedDate { get; set; }

    /// <summary>
    /// Дата последнего изменения
    /// </summary>
    public DateTime ChangedDate { get; set; }

    public virtual ICollection<Cat> Cat { get; set; } = new List<Cat>();

    public virtual ICollection<ClinicVisit> ClinicVisit { get; set; } = new List<ClinicVisit>();

    public virtual ICollection<MedSchedule> MedSchedule { get; set; } = new List<MedSchedule>();

    public virtual ICollection<VolunteerRole> VolunteerRole { get; set; } = new List<VolunteerRole>();

    public virtual ICollection<WorkTask> WorkTaskChangedBy { get; set; } = new List<WorkTask>();

    public virtual ICollection<WorkTask> WorkTaskCreatedBy { get; set; } = new List<WorkTask>();

    public virtual ICollection<WorkTaskReminder> WorkTaskReminder { get; set; } = new List<WorkTaskReminder>();

    public virtual ICollection<WorkTaskResponsible> WorkTaskResponsible { get; set; } = new List<WorkTaskResponsible>();
}
