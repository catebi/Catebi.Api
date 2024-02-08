using System;
using System.Collections.Generic;

namespace Catebi.Api.Data.Db.Entities;

/// <summary>
/// Файлы посещений
/// </summary>
public partial class ClinicVisitFile
{
    /// <summary>
    /// ID файла
    /// </summary>
    public int ClinicVisitFileId { get; set; }

    /// <summary>
    /// ID посещения
    /// </summary>
    public int? ClinicVisitId { get; set; }

    /// <summary>
    /// Имя файла
    /// </summary>
    public string FileName { get; set; } = null!;

    /// <summary>
    /// URL файла
    /// </summary>
    public string FileUrl { get; set; } = null!;

    public virtual ClinicVisit? ClinicVisit { get; set; }
}
