using AliRoyalMarquee.Domain.Common;

namespace AliRoyalMarquee.Domain.Entities;

public class PricingRule : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string RuleType { get; set; } = string.Empty;
    public decimal? FlatAmount { get; set; }
    public decimal? PercentageAmount { get; set; }
    public bool IsActive { get; set; } = true;
}
