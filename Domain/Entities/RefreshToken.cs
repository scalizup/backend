using Domain.Entities.Common;

namespace Domain.Entities;

public class RefreshToken : BaseEntity
{
    public string Token { get; set; } = default!;

    public DateTime ExpiresAt { get; set; }

    public bool IsExpired => !IsActive || DateTime.UtcNow >= ExpiresAt;

    public DateTime CreatedAt { get; set; }

    public string CreatedByIp { get; set; } = default!;

    public DateTime? RevokedAt { get; set; }

    public string? RevokedByIp { get; set; }

    public string? ReplacedByToken { get; set; }

    public bool IsActive => ReplacedByToken is null;

    public string LinkGuidJwt { get; set; } = default!;
}