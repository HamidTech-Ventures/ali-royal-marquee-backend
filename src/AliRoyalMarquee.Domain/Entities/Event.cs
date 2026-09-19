using System;
using System.Collections.Generic;
using System.Linq;
using AliRoyalMarquee.Domain.Common;

namespace AliRoyalMarquee.Domain.Entities;

public class Event : BaseEntity
{
    public string ReferenceNumber { get; private set; } = default!;
    
    public Guid BookingId { get; private set; }
    public Booking Booking { get; private set; } = default!;

    public string Title { get; private set; } = default!;
    
    public string? ManagerId { get; private set; }
    public string Status { get; private set; } = "Upcoming";

    public int ReadinessScore { get; private set; } = 0;
    public int StaffRequired { get; private set; } = 0;

    public IReadOnlyCollection<EventTask> Tasks => _tasks.AsReadOnly();
    private readonly List<EventTask> _tasks = new();

    public IReadOnlyCollection<EventStaff> Staff => _staff.AsReadOnly();
    private readonly List<EventStaff> _staff = new();

    public IReadOnlyCollection<EventMenuItem> MenuItems => _menuItems.AsReadOnly();
    private readonly List<EventMenuItem> _menuItems = new();

    private Event() { }

    public Event(Guid bookingId, string referenceNumber, string title, string? managerId)
    {
        BookingId = bookingId;
        ReferenceNumber = referenceNumber;
        Title = title;
        ManagerId = managerId;
    }

    public void UpdateDetails(string title, string? managerId, int staffRequired)
    {
        Title = title;
        ManagerId = managerId;
        StaffRequired = staffRequired;
    }

    public void UpdateStatus(string status)
    {
        Status = status;
    }

    public void UpdateReadinessScore()
    {
        if (_tasks.Count == 0)
        {
            ReadinessScore = 0;
            return;
        }

        int completed = _tasks.Count(t => t.Progress == 100);
        ReadinessScore = (int)Math.Round((double)completed / _tasks.Count * 100);
    }

    public void AddMenuItem(string name, string category, int quantity, string? notes)
    {
        _menuItems.Add(new EventMenuItem(Id, name, category, quantity, notes));
    }
}
