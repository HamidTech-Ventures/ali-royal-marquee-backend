namespace AliRoyalMarquee.Domain.Entities;

public class SystemSetting
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Key { get; set; } = null!;
    public string Value { get; set; } = null!;
    public string Category { get; set; } = null!;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
