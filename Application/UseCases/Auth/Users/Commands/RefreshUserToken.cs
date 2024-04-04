using Application.Common.Interfaces;
using FluentValidation;
using MediatR;

namespace Application.UseCases.Auth.Users.Commands;

public static class RefreshUserToken
{
    public record Command(
        string AccessToken,
        string RefreshToken) : IRequest<Dto>;

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(c => c.AccessToken)
                .NotEmpty()
                .MinimumLength(2);

            RuleFor(c => c.RefreshToken)
                .NotEmpty()
                .MinimumLength(2);
        }
    }

    public class Handler(ITokenService tokenService) : IRequestHandler<Command, Dto>
    {
        public async Task<Dto> Handle(Command request, CancellationToken cancellationToken)
        {
            var (token, refreshToken) =  await tokenService.ValidateAndRefreshTokenAsync(request.AccessToken, request.RefreshToken, cancellationToken);

            return new(token, refreshToken);
        }
    }

    public record Dto(
        string Token,
        string RefreshToken);
}