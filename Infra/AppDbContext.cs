using Domain.Entities;
using Domain.Entities.Menu;
using Microsoft.EntityFrameworkCore;

namespace Infra;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Tenant> Tenants { get; set; } = default!;

    public DbSet<TagGroup> TagGroups { get; set; } = default!;

    public DbSet<Tag> Tags { get; set; } = default!;
    
    public DbSet<Product> Products { get; set; } = default!;

    public DbSet<User> Users { get; set; } = default!;
    
    public DbSet<Role> Roles { get; set; } = default!;

    public DbSet<RefreshToken> RefreshTokens { get; set; } = default!;
    
    public DbSet<MenuSort> MenuSorts { get; set; } = default!;
}