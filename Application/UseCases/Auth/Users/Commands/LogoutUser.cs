using Application.Common.Interfaces;

namespace Application.UseCases.Auth.Users.Commands;

public static class LogoutUser
{
    public record Command : BasePermissionRequest, IRequest;

    public class Handler(
        ITokenService tokenService) : IRequestHandler<Command>
    {
        public Task Handle(Command request, CancellationToken cancellationToken)
        {
            return tokenService.RevokeRefreshTokenAsync(request.UserAccessor.RefreshToken, cancellationToken);
        }
    }
}