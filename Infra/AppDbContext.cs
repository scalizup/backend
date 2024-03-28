using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infra;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Tenant> Tenants { get; set; } = default!;
    
    public DbSet<TagGroup> TagGroups { get; set; } = default!;
}