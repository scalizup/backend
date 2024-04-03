using Application.Common.Interfaces;
using FluentValidation;
using MediatR;

namespace Application.Users.Commands;

public static class AddUserToTenant
{
    public record Command(
        string UserId,
        int TenantId) : IRequest<bool>;

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(c => c.UserId)
                .NotEmpty()
                .MinimumLength(2);

            RuleFor(c => c.TenantId)
                .GreaterThan(0);
        }
    }

    public class Handler(
        IRoleService roleService) : IRequestHandler<Command, bool>
    {
        public async Task<bool> Handle(Command request, CancellationToken cancellationToken)
        {
            var wasCreated = await roleService.AddUserToTenant(request.UserId, request.TenantId);

            return wasCreated;
        }
    }
}