using AliRoyalMarquee.Domain.Common;

namespace AliRoyalMarquee.Domain.Entities;

public class RefreshSession : BaseEntity
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    
    public string TokenHash { get; set; } = string.Empty;
    
    public DateTimeOffset ExpiresAt { get; set; }
    public DateTimeOffset? RevokedAt { get; set; }
    public DateTimeOffset? LastUsedAt { get; set; }
    
    public string? UserAgent { get; set; }
    public string? IpAddress { get; set; }
    
    // For Token Rotation: if a token is rotated, we point to the new session
    public Guid? ReplacedBySessionId { get; set; }
    public RefreshSession? ReplacedBySession { get; set; }
    
    public bool IsExpired => DateTimeOffset.UtcNow >= ExpiresAt;
    public bool IsRevoked => RevokedAt != null;
    public bool IsActive => !IsRevoked && !IsExpired;
}
