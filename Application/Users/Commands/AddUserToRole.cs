using Application.Common.Interfaces;
using FluentValidation;
using MediatR;

namespace Application.Users.Commands;

public static class AddUserToRole
{
    public record Command(
        string UserId,
        string Name) : IRequest<bool>;

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(c => c.UserId)
                .NotEmpty()
                .MinimumLength(2);

            RuleFor(c => c.Name)
                .NotEmpty()
                .MinimumLength(2);
        }
    }

    public class Handler(
        IRoleService roleService) : IRequestHandler<Command, bool>
    {
        public async Task<bool> Handle(Command request, CancellationToken cancellationToken)
        {
            var wasCreated = await roleService.AddUserToRole(request.UserId, request.Name);

            return wasCreated;
        }
    }
}