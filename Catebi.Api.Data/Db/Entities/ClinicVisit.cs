using System;
using System.Collections.Generic;

namespace Catebi.Api.Data.Db.Entities;

/// <summary>
/// Посещение врача/ветеринарной клиники
/// </summary>
public partial class ClinicVisit
{
    /// <summary>
    /// ID посещения
    /// </summary>
    public int ClinicVisitId { get; set; }

    /// <summary>
    /// ID кошки
    /// </summary>
    public int CatId { get; set; }

    /// <summary>
    /// Дата визита
    /// </summary>
    public DateOnly? VisitDate { get; set; }

    public int CompanionVolunteerId { get; set; }

    /// <summary>
    /// Название клиники
    /// </summary>
    public string? ClinicName { get; set; }

    /// <summary>
    /// Имя врача
    /// </summary>
    public string? DoctorName { get; set; }

    public virtual Cat Cat { get; set; } = null!;

    public virtual ICollection<ClinicVisitFile> ClinicVisitFile { get; set; } = new List<ClinicVisitFile>();

    public virtual Volunteer CompanionVolunteer { get; set; } = null!;

    public virtual ICollection<Prescription> Prescription { get; set; } = new List<Prescription>();
}
