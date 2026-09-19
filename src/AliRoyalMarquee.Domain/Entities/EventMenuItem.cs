using System;
using AliRoyalMarquee.Domain.Common;

namespace AliRoyalMarquee.Domain.Entities;

public class EventMenuItem : BaseEntity
{
    public Guid EventId { get; private set; }
    public Event Event { get; private set; } = default!;

    public string Name { get; private set; } = default!;
    public string Category { get; private set; } = default!; // e.g. "Main Course", "Dessert", "Starter"
    public int Quantity { get; private set; }
    public string? Notes { get; private set; }

    private EventMenuItem() { }

    public EventMenuItem(Guid eventId, string name, string category, int quantity, string? notes)
    {
        EventId = eventId;
        Name = name;
        Category = category;
        Quantity = quantity;
        Notes = notes;
    }
}
