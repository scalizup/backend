using Application.Common.Exceptions;
using Application.Repositories;
using Domain.Constants;

namespace Application.UseCases.Tenants.Commands;

public static class UpdateTenant
{
    [Authorize(Roles = [UserRoles.Admin, UserRoles.Moderator])]
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