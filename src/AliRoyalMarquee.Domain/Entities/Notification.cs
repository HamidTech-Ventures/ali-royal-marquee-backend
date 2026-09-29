namespace AliRoyalMarquee.Domain.Entities;

public class Notification
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    /// <summary>
    /// If null, the notification is a global broadcast. Otherwise, targeted to a user.
    /// </summary>
    public Guid? UserId { get; set; }
    
    public string Title { get; set; } = null!;
    public string Message { get; set; } = null!;
    
    /// <summary>
    /// "Info", "Success", "Alert", "Warning"
    /// </summary>
    public string Type { get; set; } = "Info";
    
    public bool IsRead { get; set; } = false;
    
    public string? ActionUrl { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
