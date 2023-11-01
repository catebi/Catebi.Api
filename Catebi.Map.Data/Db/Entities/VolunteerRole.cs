using System;
using System.Collections.Generic;

namespace Catebi.Map.Data.Db.Entities;

/// <summary>
/// Таблица связи &quot;волонтёр-роль&quot;
/// </summary>
public partial class VolunteerRole
{
    /// <summary>
    /// ID соотношения
    /// </summary>
    public int VolunteerRoleId { get; set; }

    /// <summary>
    /// ID роли
    /// </summary>
    public int RoleId { get; set; }

    /// <summary>
    /// ID волонтёра
    /// </summary>
    public int VolunteerId { get; set; }

    public virtual Role Role { get; set; } = null!;

    public virtual Volunteer Volunteer { get; set; } = null!;
}
