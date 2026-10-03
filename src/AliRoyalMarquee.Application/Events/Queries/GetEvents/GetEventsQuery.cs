using AliRoyalMarquee.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AliRoyalMarquee.Application.Events.Queries.GetEvents;

public class EventDto
{
    public Guid Id { get; set; }
    public Guid BookingId { get; set; }
    public string Title { get; set; } = default!;
    public string ReferenceNumber { get; set; } = default!;
    public string? ManagerId { get; set; }
    public string Status { get; set; } = default!;
    public int ReadinessScore { get; set; }
    public string DateStr { get; set; } = default!;
    public string StartTime { get; set; } = default!;
    public string EndTime { get; set; } = default!;
    public string? Hall { get; set; }
    public int Guests { get; set; }
    public List<EventStaffDto> Staff { get; set; } = new();
}

public class EventStaffDto
{
    public Guid? StaffId { get; set; }
    public string Name { get; set; }
    public string Role { get; set; }
}

public record GetEventsQuery : IRequest<List<EventDto>>;

public class GetEventsQueryHandler : IRequestHandler<GetEventsQuery, List<EventDto>>
{
    private readonly IAppDbContext _context;

    public GetEventsQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<List<EventDto>> Handle(GetEventsQuery request, CancellationToken cancellationToken)
    {
        var events = await _context.Events
            .Include(e => e.Booking)
            .ThenInclude(b => b.Venue)
            .Include(e => e.Staff)
            .AsNoTracking()
            .OrderBy(e => e.Booking.BookingDate)
            .ToListAsync(cancellationToken);

        return events.Select(e => new EventDto
        {
            Id = e.Id,
            BookingId = e.BookingId,
            Title = e.Title,
            ReferenceNumber = e.ReferenceNumber,
            ManagerId = e.ManagerId ?? "Unassigned",
            Status = e.Status,
            ReadinessScore = e.ReadinessScore,
            DateStr = e.Booking.BookingDate.ToString("MMM dd, yyyy"),
            StartTime = e.Booking.StartTime.ToString("HH:mm"),
            EndTime = e.Booking.EndTime.ToString("HH:mm"),
            Hall = e.Booking.Venue?.Name,
            Guests = e.Booking.GuestCount,
            Staff = e.Staff.Select(s => new EventStaffDto {
                StaffId = s.StaffMemberId,
                Name = s.Name,
                Role = s.Role
            }).ToList()
        }).ToList();
    }
}
