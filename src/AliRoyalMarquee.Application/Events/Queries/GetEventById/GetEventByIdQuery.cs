using AliRoyalMarquee.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AliRoyalMarquee.Application.Events.Queries.GetEventById;

public class EventDetailsDto
{
    public Guid Id { get; set; }
    public Guid BookingId { get; set; }
    public string Title { get; set; } = default!;
    public string ReferenceNumber { get; set; } = default!;
    public string? ManagerId { get; set; }
    public string Status { get; set; } = default!;
    public int ReadinessScore { get; set; }
    public int StaffRequired { get; set; }
    public string DateStr { get; set; } = default!;
    public string StartTime { get; set; } = default!;
    public string EndTime { get; set; } = default!;
    
    public string? Hall { get; set; }
    public int Guests { get; set; }
    public string? CustomerName { get; set; }
    public string? CustomerPhone { get; set; }
    public decimal TotalAmount { get; set; }

    public List<EventTaskDto> Tasks { get; set; } = new();
    public List<EventStaffDto> Staff { get; set; } = new();
    public List<EventMenuItemDto> MenuItems { get; set; } = new();
}

public class EventTaskDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = default!;
    public string? Assignee { get; set; }
    public string? DueTime { get; set; }
    public string Status { get; set; } = default!;
    public int Progress { get; set; }
}

public class EventStaffDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string Role { get; set; } = default!;
}

public class EventMenuItemDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string Category { get; set; } = default!;
    public int Quantity { get; set; }
    public string? Notes { get; set; }
}

public record GetEventByIdQuery(Guid Id) : IRequest<EventDetailsDto>;

public class GetEventByIdQueryHandler : IRequestHandler<GetEventByIdQuery, EventDetailsDto>
{
    private readonly IAppDbContext _context;

    public GetEventByIdQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<EventDetailsDto> Handle(GetEventByIdQuery request, CancellationToken cancellationToken)
    {
        var e = await _context.Events
            .Include(e => e.Booking)
                .ThenInclude(b => b.Venue)
            .Include(e => e.Booking)
                .ThenInclude(b => b.Customer)
            .Include(e => e.Tasks)
            .Include(e => e.Staff)
            .Include(e => e.MenuItems)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (e == null) return null!;

        return new EventDetailsDto
        {
            Id = e.Id,
            BookingId = e.BookingId,
            Title = e.Title,
            ReferenceNumber = e.ReferenceNumber,
            ManagerId = e.ManagerId ?? "Unassigned",
            Status = e.Status,
            ReadinessScore = e.ReadinessScore,
            StaffRequired = e.StaffRequired,
            DateStr = e.Booking.BookingDate.ToString("MMM dd, yyyy", System.Globalization.CultureInfo.InvariantCulture),
            StartTime = e.Booking.StartTime.ToString("HH:mm"),
            EndTime = e.Booking.EndTime.ToString("HH:mm"),
            Hall = e.Booking.Venue?.Name,
            Guests = e.Booking.GuestCount,
            CustomerName = e.Booking.Customer?.Name,
            CustomerPhone = e.Booking.Customer?.Phone,
            TotalAmount = e.Booking.TotalAmount,
            Tasks = e.Tasks.Select(t => new EventTaskDto
            {
                Id = t.Id,
                Title = t.Title,
                Assignee = t.Assignee,
                DueTime = t.DueTime,
                Status = t.Status,
                Progress = t.Progress
            }).ToList(),
            Staff = e.Staff.Select(s => new EventStaffDto
            {
                Id = s.Id,
                Name = s.Name,
                Role = s.Role
            }).ToList(),
            MenuItems = e.MenuItems.Select(m => new EventMenuItemDto
            {
                Id = m.Id,
                Name = m.Name,
                Category = m.Category,
                Quantity = m.Quantity,
                Notes = m.Notes
            }).ToList()
        };
    }
}
