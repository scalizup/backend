using Application.Models;
using Application.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infra.Repositories;

public class TenantRepository(
    AppDbContext context) : ITenantRepository
{
    public Task<Tenant?> GetTenantById(int id, CancellationToken cancellationToken)
    {
        var tenant = context.Tenants
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

        return tenant;
    }

    public async Task<PageQueryResponse<Tenant>> GetAllTenants(
        PageQuery pageQuery,
        CancellationToken cancellationToken)
    {
        var skip = pageQuery.PageSize * (pageQuery.PageNumber - 1);

        var tenants = await context.Tenants
            .OrderBy(t => t.Id)
            .Skip(skip)
            .Take(pageQuery.PageSize)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var totalEntities = await context.Tenants.CountAsync(cancellationToken);

        return new PageQueryResponse<Tenant>(
            totalEntities,
            pageQuery,
            tenants);
    }

    public async Task<int> CreateTenant(string name, CancellationToken cancellationToken)
    {
        var tenant = new Tenant(name);
        context.Tenants.Add(tenant);

        await context.SaveChangesAsync(cancellationToken);

        return tenant.Id;
    }

    public async Task<bool> UpdateTenant(int id, string? name, bool? isActive, CancellationToken cancellationToken)
    {
        var tenant = await context.Tenants
            .FindAsync(id, cancellationToken);

        if (tenant is null)
        {
            return false;
        }

        if (name is not null && name != tenant.Name)
        {
            tenant.Name = name;
        }

        if (isActive is not null && isActive != tenant.IsActive)
        {
            tenant.IsActive = isActive.Value;
        }
        
        if (context.Entry(tenant).State == EntityState.Unchanged)
        {
            return false;
        }

        var result = await context.SaveChangesAsync(cancellationToken);

        return result > 0;
    }

    public async Task<bool> DeleteTenant(int id, CancellationToken cancellationToken)
    {
        var tenant = await context.Tenants
            .FindAsync(id);

        if (tenant is null)
        {
            return false;
        }

        context.Tenants.Remove(tenant);

        var result = await context.SaveChangesAsync(cancellationToken);

        return result > 0;
    }
}