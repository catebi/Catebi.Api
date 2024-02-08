using System;
using System.Collections.Generic;

namespace Catebi.Api.Data.Db.Entities;

/// <summary>
/// Назначения по медицинскому уходу
/// </summary>
public partial class Prescription
{
    /// <summary>
    /// ID назначения
    /// </summary>
    public int PrescriptionId { get; set; }

    /// <summary>
    /// ID визита к врачу
    /// </summary>
    public int ClinicVisitId { get; set; }

    /// <summary>
    /// Текст назначения
    /// </summary>
    public string PrescriptionText { get; set; } = null!;

    /// <summary>
    /// Дата начала лечения
    /// </summary>
    public DateOnly StartDate { get; set; }

    /// <summary>
    /// Длительность лечения в днях
    /// </summary>
    public int? Duration { get; set; }

    /// <summary>
    /// Процедура одноразовая
    /// </summary>
    public bool OneTimeProcedure { get; set; }

    /// <summary>
    /// Периодичность, ед. изм.
    /// </summary>
    public int? PeriodicityUnitId { get; set; }

    /// <summary>
    /// Периодичность, значение
    /// </summary>
    public int? PeriodicityValue { get; set; }

    public virtual ClinicVisit ClinicVisit { get; set; } = null!;

    public virtual ICollection<MedSchedule> MedSchedule { get; set; } = new List<MedSchedule>();

    public virtual TimeUnit? PeriodicityUnit { get; set; }
}
