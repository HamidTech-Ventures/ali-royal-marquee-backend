using AliRoyalMarquee.Domain.Common;

namespace AliRoyalMarquee.Domain.Entities;

public class Role : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    
    public ICollection<User> Users { get; private set; } = new List<User>();
    public ICollection<RolePermission> RolePermissions { get; private set; } = new List<RolePermission>();
}
