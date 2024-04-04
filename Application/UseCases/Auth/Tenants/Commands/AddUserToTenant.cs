using Application.Common.Exceptions;
using Application.Repositories;
using FluentValidation;
using MediatR;

namespace Application.UseCases.Auth.Tenants.Commands;

public static class AddUserToTenant
{
    [Authorize(Role = Domain.Constants.UserRoles.Admin)]
    public record Command(
        int UserId,
        int TenantId) : IRequest<bool>;

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(c => c.UserId)
                .GreaterThan(0);

            RuleFor(c => c.TenantId)
                .GreaterThan(0);
        }
    }

    public class Handler(
        IUserRepository userRepository,
        ITenantRepository tenantRepository) : IRequestHandler<Command, bool>
    {
        public async Task<bool> Handle(Command request, CancellationToken cancellationToken)
        {
            var user = await userRepository.GetUserByIdAsync(request.UserId, cancellationToken);
            if (user is null)
            {
                throw new NotFoundException($"User with id {request.UserId} was not found.");
            }

            var tenant = await tenantRepository.GetTenantById(request.TenantId, cancellationToken);
            if (tenant is null)
            {
                throw new NotFoundException($"Tenant with id {request.TenantId} was not found.");
            }
            
            user.AvailableTenants.Add(tenant);
            return await userRepository.UpdateUserAsync(user, cancellationToken);
        }
    }
}