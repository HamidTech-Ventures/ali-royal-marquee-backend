using AliRoyalMarquee.Domain.Common;

namespace AliRoyalMarquee.Domain.Entities;

public class MenuItem : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Cost { get; set; }
    public bool IsActive { get; set; } = true;
}
