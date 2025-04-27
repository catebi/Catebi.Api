using System;
using System.Collections.Generic;

namespace Catebi.Api.Data.Db.Entities;

public partial class User
{
    public string Id { get; set; } = null!;

    public string? UserName { get; set; }

    public string? NormalizedUserName { get; set; }

    public string? Email { get; set; }

    public string? NormalizedEmail { get; set; }

    public bool EmailConfirmed { get; set; }

    public string? PasswordHash { get; set; }

    public string? SecurityStamp { get; set; }

    public string? ConcurrencyStamp { get; set; }

    public string? PhoneNumber { get; set; }

    public bool PhoneNumberConfirmed { get; set; }

    public bool TwoFactorEnabled { get; set; }

    public DateTime? LockoutEnd { get; set; }

    public bool LockoutEnabled { get; set; }

    public int AccessFailedCount { get; set; }

    public virtual ICollection<UserClaim> UserClaim { get; set; } = new List<UserClaim>();

    public virtual ICollection<UserLogin> UserLogin { get; set; } = new List<UserLogin>();

    public virtual ICollection<UserToken> UserToken { get; set; } = new List<UserToken>();

    public virtual ICollection<Role> Role { get; set; } = new List<Role>();
}
