using Domain.Entities;
using Domain.Utils;

namespace Domain.Repositories;

public interface ITenantRepository
{
    Task<Tenant?> GetTenantById(int id, CancellationToken cancellationToken);

    Task<PageQueryResponse<Tenant>> GetAllTenants(
        PageQuery pageQuery,
        CancellationToken cancellationToken);

    Task<int> CreateTenant(string name, CancellationToken cancellationToken);

    Task<bool> UpdateTenant(int id, string? name, bool? isActive, CancellationToken cancellationToken);

    Task<bool> DeleteTenant(int id, CancellationToken cancellationToken);
}