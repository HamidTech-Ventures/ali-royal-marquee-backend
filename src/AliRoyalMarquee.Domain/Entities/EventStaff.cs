using System;
using AliRoyalMarquee.Domain.Common;

namespace AliRoyalMarquee.Domain.Entities;

public class EventStaff : BaseEntity
{
    public Guid EventId { get; private set; }
    public Event Event { get; private set; } = default!;

    public string Name { get; private set; } = default!;
    public string Role { get; private set; } = default!; // e.g. "Server", "Security", "Coordinator"
    
    public Guid? StaffMemberId { get; private set; }
    public StaffMember? StaffMember { get; private set; }

    private EventStaff() { }

    public EventStaff(Guid eventId, string name, string role, Guid? staffMemberId = null)
    {
        EventId = eventId;
        Name = name;
        Role = role;
        StaffMemberId = staffMemberId;
    }
}
