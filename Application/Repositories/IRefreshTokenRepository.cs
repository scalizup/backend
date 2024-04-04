using Domain.Entities;

namespace Application.Repositories;

public interface IRefreshTokenRepository
{
    Task<RefreshToken?> GetRefreshToken(string token, CancellationToken cancellationToken);

    Task<RefreshToken> CreateRefreshToken(RefreshToken refreshToken, CancellationToken cancellationToken);

    Task<bool> UpdateRefreshToken(RefreshToken refreshToken, CancellationToken cancellationToken);
}