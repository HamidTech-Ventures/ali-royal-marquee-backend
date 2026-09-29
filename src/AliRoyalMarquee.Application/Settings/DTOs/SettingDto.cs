namespace AliRoyalMarquee.Application.Settings.DTOs;

public class SettingDto
{
    public Guid Id { get; set; }
    public string Key { get; set; } = null!;
    public string Value { get; set; } = null!;
    public string Category { get; set; } = null!;
}
