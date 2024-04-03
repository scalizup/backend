using Domain.Entities;
using Infra.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infra;

public class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<Tenant> Tenants { get; set; } = default!;

    public DbSet<TagGroup> TagGroups { get; set; } = default!;

    public DbSet<Tag> Tags { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<ApplicationUser>()
            .HasMany(u => u.AvailableTenants)
            .WithMany();

        base.OnModelCreating(builder);
    }
}