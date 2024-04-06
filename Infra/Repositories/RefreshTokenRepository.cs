using Application.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infra.Repositories;

public class RefreshTokenRepository(AppDbContext context) : IRefreshTokenRepository
{
    public async Task<RefreshToken?> GetRefreshToken(string token, CancellationToken cancellationToken)
    {
        return await context.RefreshTokens
            .FirstOrDefaultAsync(t => t.Token == token, cancellationToken);
    }

    public async Task<RefreshToken> CreateRefreshToken(RefreshToken refreshToken, CancellationToken cancellationToken)
    {
        await context.RefreshTokens.AddAsync(refreshToken, cancellationToken);

        await context.SaveChangesAsync(cancellationToken);

        return refreshToken;
    }

    public async Task<bool> UpdateRefreshToken(RefreshToken refreshToken, CancellationToken cancellationToken)
    {
        context.RefreshTokens.Update(refreshToken);

        var result = await context.SaveChangesAsync(cancellationToken);

        return result > 0;
    }

    public async Task<bool> CheckIfTokenIsBlacklistedAsync(string token, CancellationToken cancellationToken)
    {
        return await context.RefreshTokens 
            .AnyAsync(t => t.Token == token && t.IsBlacklisted, cancellationToken);
    }
}