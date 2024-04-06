using Application.Common.Exceptions;
using Application.Repositories;
using FluentValidation;
using MediatR;

namespace Application.UseCases.Auth.Roles.Commands;

public static class AddUserToRole
{
    // [Authorize(Role = Domain.Constants.UserRoles.Admin)]
    public record Command(
        int UserId,
        string Name) : IRequest<bool>;

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(c => c.UserId)
                .GreaterThan(0);

            RuleFor(c => c.Name)
                .NotEmpty()
                .MinimumLength(2);
        }
    }

    public class Handler(
        IUserRepository userRepository,
        IRoleRepository roleRepository) : IRequestHandler<Command, bool>
    {
        public async Task<bool> Handle(Command request, CancellationToken cancellationToken)
        {
            var user = await userRepository.GetUserByIdAsync(request.UserId, cancellationToken);
            if (user is null)
            {
                throw new NotFoundException($"User with id {request.UserId} was not found.");
            }

            if (user.Roles.Any(r => r.Name == Domain.Constants.UserRoles.Admin))
            {
                throw new ForbiddenAccessException("Admins cannot be added to roles.");
            }

            if (user.Roles.Any(r => r.Name == request.Name))
            {
                throw new ForbiddenAccessException("User is already in this role.");
            }

            var role = await roleRepository.GetRoleByNameAsync(request.Name, cancellationToken);
            if (role is null)
            {
                throw new NotFoundException($"Role with name {request.Name} was not found.");
            }

            return await roleRepository.AddRoleToUserAsync(request.UserId, role.Id, cancellationToken);
        }
    }
}