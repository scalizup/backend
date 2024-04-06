using Application.Repositories;
using FluentValidation;
using MediatR;

namespace Application.UseCases.Auth.Roles.Commands;

public static class CreateRole
{
    // [Authorize(Role = Domain.Constants.UserRoles.Admin)]
    public record Command(
        string Name) : IRequest<int>;

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(c => c.Name)
                .NotEmpty()
                .MinimumLength(2);
        }
    }

    public class Handler(IRoleRepository roleRepository) : IRequestHandler<Command, int>
    {
        public async Task<int> Handle(Command request, CancellationToken cancellationToken)
        {
            var result = await roleRepository.CreateRoleAsync(new(request.Name), cancellationToken);

            return result;
        }
    }
}