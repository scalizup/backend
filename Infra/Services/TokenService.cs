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
    IUser userRequest,
    IOptions<JwtConfiguration> jwtConfiguration)
    : ITokenService
{
    private readonly JwtSecurityTokenHandler _tokenHandler = new();
    private readonly JwtConfiguration _jwtConfiguration = jwtConfiguration.Value;

    private string GenerateRefreshToken()
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
            Expires = DateTime.UtcNow.AddMinutes(15),
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
            CreatedByIp = userRequest.RequestIp,
            LinkGuidJwt = linkGuid
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
        if (refreshTokenEntity is null || refreshTokenEntity.IsBlacklisted || refreshTokenEntity.IsExpired || linkGuid != refreshTokenEntity.LinkGuidJwt)
        {
            throw new Exception("Refresh token is invalid, please login again.");
        }
        
        var user = await userRepository.GetUserByIdAsync(userId, cancellationToken);
        var (newToken, newRefreshToken) = await GenerateToken(user!, cancellationToken);

        refreshTokenEntity.RevokedAt = DateTime.UtcNow;
        refreshTokenEntity.RevokedByIp = userRequest.RequestIp;
        refreshTokenEntity.ReplacedByToken = newRefreshToken;

        await refreshTokenRepository.UpdateRefreshToken(refreshTokenEntity, cancellationToken);

        return (newToken, newRefreshToken);
    }
}