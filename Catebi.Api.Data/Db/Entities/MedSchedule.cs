using System;
using System.Collections.Generic;

namespace Catebi.Api.Data.Db.Entities;

/// <summary>
/// График медицинского ухода
/// </summary>
public partial class MedSchedule
{
    /// <summary>
    /// ID записи в графике(журнале) мед. ухода
    /// </summary>
    public int MedScheduleRecordId { get; set; }

    /// <summary>
    /// ID кошки
    /// </summary>
    public int CatId { get; set; }

    /// <summary>
    /// ID назначения
    /// </summary>
    public int PrescriptionId { get; set; }

    /// <summary>
    /// Дата и время назначенной процедуры
    /// </summary>
    public DateTime ProcedureTime { get; set; }

    /// <summary>
    /// Процедура выполнена
    /// </summary>
    public bool Done { get; set; }

    /// <summary>
    /// Волонтёр-исполнитель
    /// </summary>
    public int? VolunteerId { get; set; }

    public virtual Cat Cat { get; set; } = null!;

    public virtual Prescription Prescription { get; set; } = null!;

    public virtual Volunteer? Volunteer { get; set; }
}
