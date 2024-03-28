namespace Domain.UseCases.Tenant.Commands;

public static class UpdateTenant
{
    public record Command( 
        int Id,
        string? Name,
        bool? IsActive) : IRequest<bool>;

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(c => c.Id)
                .GreaterThan(0);

            RuleFor(c => c.Name)
                .NotEmpty()
                .MinimumLength(2)
                .When(t => t.Name is not null);
        }
    }

    public class Handler(ITenantRepository tenantRepository) : IRequestHandler<Command, bool>
    {
        public async Task<bool> Handle(Command request, CancellationToken cancellationToken)
        {
            var tenant = await tenantRepository.GetTenantById(request.Id, cancellationToken);
            
            if (tenant is null)
            {
                throw new NotFoundException($"Tenant with id {request.Id} was not found.");
            }
            
            var wasUpdated = await tenantRepository.UpdateTenant(
                request.Id,
                request.Name,
                request.IsActive,
                cancellationToken);

            return wasUpdated;
        }
    }
}