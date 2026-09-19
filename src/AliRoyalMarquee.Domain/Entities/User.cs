using AliRoyalMarquee.Domain.Common;
using AliRoyalMarquee.Domain.Enums;

namespace AliRoyalMarquee.Domain.Entities;

public class User : BaseEntity
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public UserStatus Status { get; set; } = UserStatus.Active;
    
    public int FailedLoginCount { get; set; }
    public DateTimeOffset? LockoutUntil { get; set; }
    public DateTimeOffset? LastLoginAt { get; set; }
    
    public Guid RoleId { get; set; }
    public Role Role { get; set; } = null!;
    
    public ICollection<RefreshSession> RefreshSessions { get; private set; } = new List<RefreshSession>();
}
