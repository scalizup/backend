using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Repositories;
using FluentValidation;
using MediatR;

namespace Application.UseCases.Auth.Users.Commands;

public static class LoginUser
{
    public record Command(
        string Username,
        string Password) : IRequest<Dto>;

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
        IUserRepository userRepository,
        ITokenService tokenService) : IRequestHandler<Command, Dto>
    {
        public async Task<Dto> Handle(Command request, CancellationToken cancellationToken)
        {
            var user = await userRepository.GetUserByUsernameAsync(request.Username.ToLower(), cancellationToken);
            if (user is null || !string.Equals(request.Password, user.Password))
            {
                throw new ForbiddenAccessException("Invalid username or password.");
            }

            var (accessToken, refreshToken) = await tokenService.GenerateToken(user, cancellationToken);

            return new(accessToken, refreshToken);
        }
    }

    public record Dto(
        string AccessToken,
        string RefreshToken);
}