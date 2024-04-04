using Domain.Entities;

namespace Application.Common.Interfaces;

public interface ITokenService
{
    Task<(string Token, string RefreshToken)> GenerateToken(User user, CancellationToken cancellationToken);
    
    Task<(string Token, string RefreshToken)> ValidateAndRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken);
}