using System;
using System.Collections.Generic;

namespace Catebi.Api.Data.Db.Entities;

/// <summary>
/// Stores file data with metadata including original filename, content type, and upload timestamp
/// </summary>
public partial class FileStorage
{
    /// <summary>
    /// Unique identifier for the file, automatically generated using gen_random_uuid()
    /// </summary>
    public Guid FileStorageId { get; set; }

    /// <summary>
    /// Original filename of the uploaded file
    /// </summary>
    public string FileName { get; set; } = null!;

    /// <summary>
    /// MIME type of the file (e.g., image/jpeg, application/pdf)
    /// </summary>
    public string ContentType { get; set; } = null!;

    /// <summary>
    /// Binary data of the file stored as BYTEA
    /// </summary>
    public byte[] Content { get; set; } = null!;

    /// <summary>
    /// Size of the file in bytes
    /// </summary>
    public long Size { get; set; }

    /// <summary>
    /// Timestamp when the file was uploaded, automatically set to current time
    /// </summary>
    public DateTime? Created { get; set; }
}
