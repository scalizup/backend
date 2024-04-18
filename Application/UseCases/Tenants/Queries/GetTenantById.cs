using Application.Common.Exceptions;
using Application.Repositories;

namespace Application.UseCases.Tenants.Queries;

public static class GetTenantById
{
    public record Query : BasePermissionRequest, IRequest<TenantDto>;
    
    public class Handler(ITenantRepository tenantRepository) : IRequestHandler<Query, TenantDto>
    {
        public async Task<TenantDto> Handle(Query request, CancellationToken cancellationToken)
        {
            var tenant = await tenantRepository.GetTenantById(request.TenantId, cancellationToken);
            if (tenant is null)
            {
                throw new NotFoundException($"Tenant with id {request.TenantId} not found.");
            }

            return new TenantDto(tenant.Id, tenant.Name, tenant.IsActive);
        }
    }

    public record TenantDto(
        int Id,
        string Name,
        bool IsActive);
}