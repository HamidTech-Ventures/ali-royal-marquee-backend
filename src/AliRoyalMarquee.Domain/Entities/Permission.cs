using AliRoyalMarquee.Domain.Common;

namespace AliRoyalMarquee.Domain.Entities;

public class Permission : BaseEntity
{
    public string Name { get; set; } = string.Empty; // e.g. "View", "Create", "Approve"
    
    public ICollection<RolePermission> RolePermissions { get; private set; } = new List<RolePermission>();
}
