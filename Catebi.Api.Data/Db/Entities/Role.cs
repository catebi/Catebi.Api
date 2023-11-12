using System;
using System.Collections.Generic;

namespace Catebi.Api.Data.Db.Entities;

/// <summary>
/// Список ролей волонтёров
/// </summary>
public partial class Role
{
    /// <summary>
    /// ID роли
    /// </summary>
    public int RoleId { get; set; }

    /// <summary>
    /// Наименование роли
    /// </summary>
    public string? Name { get; set; }

    public virtual ICollection<RolePermission> RolePermission { get; set; } = new List<RolePermission>();

    public virtual ICollection<VolunteerRole> VolunteerRole { get; set; } = new List<VolunteerRole>();
}
