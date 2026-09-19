using System;
using AliRoyalMarquee.Domain.Common;

namespace AliRoyalMarquee.Domain.Entities;

public class EventTask : BaseEntity
{
    public Guid EventId { get; private set; }
    public Event Event { get; private set; } = default!;

    public string Title { get; private set; } = default!;
    public string? Assignee { get; private set; }
    public string? DueTime { get; private set; } // e.g. "14:00"
    
    // Status can be: Pending, In Progress, Completed
    public string Status { get; private set; } = "Pending";
    public int Progress { get; private set; } = 0;

    private EventTask() { }

    public EventTask(Guid eventId, string title, string? assignee, string? dueTime)
    {
        EventId = eventId;
        Title = title;
        Assignee = assignee;
        DueTime = dueTime;
    }

    public void UpdateStatus(string status, int progress)
    {
        Status = status;
        Progress = progress;
    }
}
