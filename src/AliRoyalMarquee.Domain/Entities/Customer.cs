using System;
using System.Collections.Generic;
using AliRoyalMarquee.Domain.Common;
using AliRoyalMarquee.Domain.Enums;

namespace AliRoyalMarquee.Domain.Entities;

public class Customer : BaseEntity
{
    public string Name { get; private set; } = default!;
    public string Phone { get; private set; } = default!;
    public string? Email { get; private set; }
    public CustomerTier Tier { get; private set; }
    
    // Navigation properties
    public IReadOnlyCollection<Enquiry> Enquiries => _enquiries.AsReadOnly();
    private readonly List<Enquiry> _enquiries = new();

    public IReadOnlyCollection<Booking> Bookings => _bookings.AsReadOnly();
    private readonly List<Booking> _bookings = new();

    private Customer() { } // EF Core

    public Customer(string name, string phone, string? email, CustomerTier tier)
    {
        Name = name;
        Phone = phone;
        Email = email;
        Tier = tier;
    }

    public void UpdateDetails(string name, string phone, string? email, CustomerTier tier)
    {
        Name = name;
        Phone = phone;
        Email = email;
        Tier = tier;
    }
}
