using Application.Common.Interfaces;
using FluentValidation;
using MediatR;

namespace Application.Users.Commands;

public static class CreateRole
{
    public record Command(
        string Name) : IRequest<bool>;

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(c => c.Name)
                .NotEmpty()
                .MinimumLength(2);
        }
    }

    public class Handler(IRoleService roleService) : IRequestHandler<Command, bool>
    {
        public async Task<bool> Handle(Command request, CancellationToken cancellationToken)
        {
            var wasCreated = await roleService.CreateRole(request.Name);

            return wasCreated;
        }
    }
}