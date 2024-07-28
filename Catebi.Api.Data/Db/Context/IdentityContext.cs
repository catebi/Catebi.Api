using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

public class IdentityContext : IdentityDbContext<IdentityUser>
{
    public IdentityContext(DbContextOptions<IdentityContext> options) :
        base(options)
    { }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) =>
        optionsBuilder
            .UseSnakeCaseNamingConvention();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema("identity");

        //Configure default schema
        modelBuilder.Entity<IdentityUser>().ToTable("user");
        modelBuilder.Entity<IdentityUserToken<string>>().ToTable("user_token");
        modelBuilder.Entity<IdentityUserLogin<string>>().ToTable("user_login");
        modelBuilder.Entity<IdentityUserClaim<string>>().ToTable("user_claim");
        modelBuilder.Entity<IdentityRole>().ToTable("role");
        modelBuilder.Entity<IdentityUserRole<string>>().ToTable("user_role");
        modelBuilder.Entity<IdentityRoleClaim<string>>().ToTable("role_claim");
    }
}