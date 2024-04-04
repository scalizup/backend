using Application.Common.Exceptions;
using Application.Repositories;
using Domain.Entities;
using FluentValidation;
using MediatR;

namespace Application.UseCases.Auth.Users.Commands;

public static class RegisterUser
{
    public record Command(
        string Username,
        string Password) : IRequest<int>;

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(c => c.Username)
                .NotEmpty()
                .MinimumLength(2);

            RuleFor(c => c.Password)
                .NotEmpty()
                .MinimumLength(2);
        }
    }

    public class Handler(
        IUserRepository userRepository) : IRequestHandler<Command, int>
    {
        public async Task<int> Handle(Command request, CancellationToken cancellationToken)
        {
            var user = await userRepository.GetUserByUsernameAsync(request.Username.ToLower(), cancellationToken);
            if (user is not null)
            {
                throw new ForbiddenAccessException("User with this username already exists.");
            }

            var newUser = new User(request.Username, request.Password);
            await userRepository.CreateUserAsync(newUser, cancellationToken);

            return newUser.Id;
        }
    }
}