using System;
using System.Collections.Generic;

namespace Catebi.Api.Data.Db.Entities;

public partial class RoleClaim
{
    public int Id { get; set; }

    public string RoleId { get; set; } = null!;

    public string? ClaimType { get; set; }

    public string? ClaimValue { get; set; }

    public virtual Role Role { get; set; } = null!;
}
