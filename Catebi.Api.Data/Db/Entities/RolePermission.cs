using System;
using System.Collections.Generic;

namespace Catebi.Api.Data.Db.Entities;

/// <summary>
/// Таблица связи роли с разрешениями
/// </summary>
public partial class RolePermission
{
    /// <summary>
    /// ID соотношения
    /// </summary>
    public int RolePermissionId { get; set; }

    /// <summary>
    /// ID роли
    /// </summary>
    public int RoleId { get; set; }

    /// <summary>
    /// ID разрешения
    /// </summary>
    public int PermissionId { get; set; }

    public virtual Permission Permission { get; set; } = null!;

    public virtual Role Role { get; set; } = null!;
}
