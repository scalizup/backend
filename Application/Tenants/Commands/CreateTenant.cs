using Application.Repositories;
using FluentValidation;
using MediatR;

namespace Application.Tenants.Commands;

public static class CreateTenant
{
    public record Command( 
        string Name) : IRequest<int>;

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .MinimumLength(2);
        }
    }

    public class Handler(ITenantRepository tenantRepository) : IRequestHandler<Command, int>
    {
        public async Task<int> Handle(Command request, CancellationToken cancellationToken)
        {
            var newTenant = await tenantRepository.CreateTenant(request.Name, cancellationToken);

            return newTenant;
        }
    }
}