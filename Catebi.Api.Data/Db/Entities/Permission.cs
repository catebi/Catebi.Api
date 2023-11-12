using System;
using System.Collections.Generic;

namespace Catebi.Api.Data.Db.Entities;

/// <summary>
/// Список прав (разрешений/доступов)
/// </summary>
public partial class Permission
{
    /// <summary>
    /// ID права
    /// </summary>
    public int PermissionId { get; set; }

    /// <summary>
    /// Наименование права
    /// </summary>
    public string? Name { get; set; }

    public virtual ICollection<RolePermission> RolePermission { get; set; } = new List<RolePermission>();
}
