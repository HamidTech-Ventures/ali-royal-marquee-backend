using AliRoyalMarquee.Domain.Common;

namespace AliRoyalMarquee.Domain.Entities;

public class Addon : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string? Description { get; set; }
    public string Unit { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
}
