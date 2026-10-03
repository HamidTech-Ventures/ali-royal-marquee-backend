using AliRoyalMarquee.Application.Bookings.DTOs;
using AliRoyalMarquee.Application.Common.Interfaces;
using AliRoyalMarquee.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using System.Collections.Generic;

namespace AliRoyalMarquee.Application.Bookings.Queries.GetBookings;

public class GetBookingsResponse
{
    public List<BookingListDto> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
}

public record GetBookingsQuery : IRequest<GetBookingsResponse>
{
    public string? SearchTerm { get; init; }
    public string? Status { get; init; }
    public DateTime? StartDate { get; init; }
    public DateTime? EndDate { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 50;
}

public class GetBookingsQueryHandler : IRequestHandler<GetBookingsQuery, GetBookingsResponse>
{
    private readonly IAppDbContext _context;

    public GetBookingsQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<GetBookingsResponse> Handle(GetBookingsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Bookings
            .Include(b => b.Customer)
            .Include(b => b.Venue)
            .Include(b => b.Event)
            .Include(b => b.Payments)
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.Status) && request.Status != "All")
        {
            if (Enum.TryParse<BookingStatus>(request.Status, true, out var statusEnum))
            {
                query = query.Where(b => b.Status == statusEnum);
            }
        }

        if (request.StartDate.HasValue)
        {
            var start = request.StartDate.Value.Date;
            query = query.Where(b => b.BookingDate >= start);
        }

        if (request.EndDate.HasValue)
        {
            var end = request.EndDate.Value.Date;
            query = query.Where(b => b.BookingDate <= end);
        }

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var search = request.SearchTerm.ToLower();
            query = query.Where(b => 
                b.ReferenceNumber.ToLower().Contains(search) ||
                b.Customer.Name.ToLower().Contains(search) ||
                b.Customer.Phone.Contains(search) ||
                b.Venue.Name.ToLower().Contains(search)
            );
        }

        query = query.OrderByDescending(b => b.BookingDate).ThenByDescending(b => b.StartTime);

        var totalCount = await query.CountAsync(cancellationToken);
        
        var items = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(b => new BookingListDto
            {
                Id = b.Id,
                ReferenceNumber = b.ReferenceNumber,
                CustomerId = b.CustomerId,
                CustomerName = b.Customer.Name,
                CustomerPhone = b.Customer.Phone,
                EventId = b.Event != null ? (Guid?)b.Event.Id : null,
                EventTitle = b.Event != null ? b.Event.Title : null,
                VenueId = b.VenueId,
                Hall = b.Venue.Name,
                DateStr = b.BookingDate.ToString("MMM dd, yyyy", System.Globalization.CultureInfo.InvariantCulture),
                Shift = b.StartTime.Hour < 17 ? "Day" : "Night",
                Guests = b.GuestCount,
                TotalAmount = b.TotalAmount,
                PaidAmount = b.Payments.Where(p => p.Status == PaymentStatus.Completed).Sum(p => p.Amount),
                Status = b.Status.ToString(),
                PaymentStatus = GetPaymentStatus(b.TotalAmount, b.Payments.Where(p => p.Status == PaymentStatus.Completed).Sum(p => p.Amount)),
                CreatedAt = b.CreatedAt
            })
            .ToListAsync(cancellationToken);

        return new GetBookingsResponse
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }

    private static string GetPaymentStatus(decimal total, decimal paid)
    {
        if (paid >= total) return "Paid";
        if (paid > 0) return "Partial";
        return "Unpaid";
    }
}
