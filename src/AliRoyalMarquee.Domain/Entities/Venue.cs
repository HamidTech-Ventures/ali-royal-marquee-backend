using System;
using AliRoyalMarquee.Domain.Common;

namespace AliRoyalMarquee.Domain.Entities;

public class Venue : BaseEntity
{
    public string Name { get; private set; } = default!;
    public int Capacity { get; private set; }
    public string? Description { get; private set; }
    public bool IsActive { get; private set; }

    private Venue() { } // EF Core

    public Venue(string name, int capacity, string? description)
    {
        Name = name;
        Capacity = capacity;
        Description = description;
        IsActive = true;
    }
    
    
    public void UpdateDetails(string name, int capacity, string? description)
    {
        Name = name;
        Capacity = capacity;
        Description = description;
    }
    public void UpdateStatus(bool isActive)
    {
        IsActive = isActive;
    }
}
