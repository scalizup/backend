using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Application.Common.Interfaces;
using Application.Repositories;
using Domain.Entities;
using Infra.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Infra.Services;

public class TokenService(
    IRefreshTokenRepository refreshTokenRepository,
    IUserRepository userRepository,
    IUserAccessor userAccessorRequest,
    IOptions<JwtConfiguration> jwtConfiguration)
    : ITokenService
{
    private readonly JwtSecurityTokenHandler _tokenHandler = new();
    private readonly JwtConfiguration _jwtConfiguration = jwtConfiguration.Value;

    private static string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    public async Task<(string Token, string RefreshToken)> GenerateToken(User user, CancellationToken cancellationToken)
    {
        var refreshTokenLinkGuid = GenerateRefreshToken();

        var linkGuid = Guid.NewGuid().ToString();

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim("linkGuid", linkGuid),
            }),
            Expires = DateTime.UtcNow.AddSeconds(30),
            SigningCredentials =
                new SigningCredentials(_jwtConfiguration.SignedKey, SecurityAlgorithms.HmacSha256),
            Issuer = _jwtConfiguration.Issuer,
            Audience = _jwtConfiguration.Audience,
        };

        var token = _tokenHandler.CreateToken(tokenDescriptor);

        var refreshToken = new RefreshToken
        {
            Token = refreshTokenLinkGuid,
            ExpiresAt = DateTime.UtcNow.AddMonths(1),
            CreatedAt = DateTime.UtcNow,
            CreatedByIp = userAccessorRequest.RequestIp,
            LinkGuidJwt = linkGuid,
            TenantId = user.AvailableTenants.First().Id,
        };

        await refreshTokenRepository.CreateRefreshToken(refreshToken, cancellationToken);

        return (_tokenHandler.WriteToken(token), refreshTokenLinkGuid);
    }

    public async Task<(string Token, string RefreshToken)> ValidateAndRefreshTokenAsync(string token,
        string refreshToken, CancellationToken cancellationToken)
    {
        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = false,
            ValidateIssuerSigningKey = true,
            ValidIssuer = _jwtConfiguration.Issuer,
            ValidAudience = _jwtConfiguration.Audience,
            IssuerSigningKey = _jwtConfiguration.SignedKey,
        };

        if (token.Contains("Bearer "))
        {
            token = token.Replace("Bearer ", "");
        }

        var result = await _tokenHandler.ValidateTokenAsync(token, validationParameters);
        var userIdClaim = result.Claims[ClaimTypes.NameIdentifier] as string;
        if (!int.TryParse(userIdClaim, out var userId) || result.Claims["linkGuid"] is not string linkGuid)
        {
            throw new Exception("Refresh token is invalid, please login again.");
        }

        var refreshTokenEntity = await refreshTokenRepository.GetRefreshToken(refreshToken, cancellationToken);
        if (refreshTokenEntity is null || refreshTokenEntity.IsBlacklisted || refreshTokenEntity.IsExpired ||
            linkGuid != refreshTokenEntity.LinkGuidJwt)
        {
            throw new Exception("Refresh token is invalid, please login again.");
        }

        var user = await userRepository.GetUserByIdAsync(userId, cancellationToken);
        var (newToken, newRefreshToken) = await GenerateToken(user!, cancellationToken);

        refreshTokenEntity.RevokedAt = DateTime.UtcNow;
        refreshTokenEntity.RevokedByIp = userAccessorRequest.RequestIp;
        refreshTokenEntity.ReplacedByToken = newRefreshToken;

        await refreshTokenRepository.UpdateRefreshToken(refreshTokenEntity, cancellationToken);

        return (newToken, newRefreshToken);
    }

    public async Task RevokeRefreshTokenAsync(RefreshToken refreshToken, CancellationToken cancellationToken)
    {
        refreshToken.RevokedAt = DateTime.UtcNow;
        refreshToken.RevokedByIp = userAccessorRequest.RequestIp;

        await refreshTokenRepository.UpdateRefreshToken(refreshToken, cancellationToken);
    }
}