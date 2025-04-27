using System;
using System.Collections.Generic;

namespace Catebi.Api.Data.Db.Entities;

public partial class Role
{
    public string Id { get; set; } = null!;

    public string? Name { get; set; }

    public string? NormalizedName { get; set; }

    public string? ConcurrencyStamp { get; set; }

    public virtual ICollection<RoleClaim> RoleClaim { get; set; } = new List<RoleClaim>();

    public virtual ICollection<User> User { get; set; } = new List<User>();
}
