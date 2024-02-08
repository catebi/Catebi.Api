using System;
using System.Collections.Generic;

namespace Catebi.Api.Data.Db.Entities;

/// <summary>
/// Кошка/кот
/// </summary>
public partial class Cat
{
    /// <summary>
    /// Id кошки в бд
    /// </summary>
    public int CatId { get; set; }

    /// <summary>
    /// Id в Notion
    /// </summary>
    public string? NotionCatId { get; set; }

    /// <summary>
    /// Имя/описание
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// Адрес (где нашли кошку)
    /// </summary>
    public string? Address { get; set; }

    /// <summary>
    /// Геолокация (координаты по адресу)
    /// </summary>
    public string? GeoLocation { get; set; }

    /// <summary>
    /// Ссылка на страницу в Notion
    /// </summary>
    public string? NotionPageUrl { get; set; }

    /// <summary>
    /// Пол
    /// </summary>
    public int CatSexId { get; set; }

    /// <summary>
    /// Ошейник
    /// </summary>
    public int? CatCollarId { get; set; }

    /// <summary>
    /// Id комнаты в котодоме
    /// </summary>
    public int? CatHouseSpaceId { get; set; }

    /// <summary>
    /// Дата прибытия кошки
    /// </summary>
    public DateOnly? InDate { get; set; }

    /// <summary>
    /// Дата отъезда кошки
    /// </summary>
    public DateOnly? OutDate { get; set; }

    /// <summary>
    /// Дата стерилизации кошки
    /// </summary>
    public DateOnly? NeuteredDate { get; set; }

    /// <summary>
    /// Id волонтёра, ответственного за кошку (в notion - deliverer)
    /// </summary>
    public int? ResponsibleVolunteerId { get; set; }

    /// <summary>
    /// Текст примечания
    /// </summary>
    public string? Comment { get; set; }

    /// <summary>
    /// Дата создания
    /// </summary>
    public DateTime CreatedDate { get; set; }

    /// <summary>
    /// Дата последнего изменения
    /// </summary>
    public DateTime ChangedDate { get; set; }

    public virtual ICollection<CatCatTag> CatCatTag { get; set; } = new List<CatCatTag>();

    public virtual CatCollar? CatCollar { get; set; }

    public virtual CatHouseSpace? CatHouseSpace { get; set; }

    public virtual ICollection<CatImageUrl> CatImageUrl { get; set; } = new List<CatImageUrl>();

    public virtual CatSex CatSex { get; set; } = null!;

    public virtual ICollection<ClinicVisit> ClinicVisit { get; set; } = new List<ClinicVisit>();

    public virtual ICollection<MedSchedule> MedSchedule { get; set; } = new List<MedSchedule>();

    public virtual Volunteer? ResponsibleVolunteer { get; set; }
}
