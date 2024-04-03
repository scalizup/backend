using Application.Models;
using Application.Repositories;
using MediatR;

namespace Application.Tenants.Queries;

public static class GetAllTenants
{
    [Authorize(Roles = "Admin")]
    public record Query(
        PageQuery PageQuery) : IRequest<PageQueryResponse<TenantDto>>;

    public class Handler(ITenantRepository tenantRepository)
        : IRequestHandler<Query, PageQueryResponse<TenantDto>>
    {
        public async Task<PageQueryResponse<TenantDto>> Handle(
            Query request,
            CancellationToken cancellationToken)
        {
            var response = await tenantRepository.GetAllTenants(
                request.PageQuery,
                cancellationToken);

            return response.Transform(t => new TenantDto(t.Id, t.Name, t.IsActive));
        }
    }

    public record TenantDto(
        int Id,
        string Name,
        bool IsActive);
}