namespace Domain.UseCases.Tenant;

public static class GetTenantById
{
    public record Query(int Id) : IRequest<TenantDto>;
    
    public class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0);
        }
    }

    public class Handler(ITenantRepository tenantRepository) : IRequestHandler<Query, TenantDto>
    {
        public async Task<TenantDto> Handle(Query request, CancellationToken cancellationToken)
        {
            var tenant = await tenantRepository.GetTenantById(request.Id, cancellationToken);
            if (tenant is null)
            {
                throw new NotFoundException($"Tenant with id {request.Id} not found.");
            }

            return new TenantDto(tenant.Id, tenant.Name, tenant.IsActive);
        }
    }

    public record TenantDto(
        int Id,
        string Name,
        bool IsActive);
}