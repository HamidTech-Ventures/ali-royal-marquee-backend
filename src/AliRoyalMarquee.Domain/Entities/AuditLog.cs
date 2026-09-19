using System;
using AliRoyalMarquee.Domain.Common;

namespace AliRoyalMarquee.Domain.Entities;

public class AuditLog : BaseEntity
{
    public Guid UserId { get; private set; }
    public string Action { get; private set; } = default!;
    public string? EntityType { get; private set; }
    public string? EntityId { get; private set; }
    public string Description { get; private set; } = default!;
    public DateTime Timestamp { get; private set; }
    public string? Metadata { get; private set; }

    private AuditLog() { }

    public AuditLog(Guid userId, string action, string? entityType, string? entityId, string description, string? metadata = null)
    {
        UserId = userId;
        Action = action;
        EntityType = entityType;
        EntityId = entityId;
        Description = description;
        Timestamp = DateTime.UtcNow;
        Metadata = metadata;
    }
}
