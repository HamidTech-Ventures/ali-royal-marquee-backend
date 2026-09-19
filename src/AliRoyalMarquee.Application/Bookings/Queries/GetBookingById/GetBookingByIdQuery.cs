using AliRoyalMarquee.Application.Bookings.DTOs;
using AliRoyalMarquee.Application.Common.Interfaces;
using AliRoyalMarquee.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AliRoyalMarquee.Application.Bookings.Queries.GetBookingById;

public record GetBookingByIdQuery(Guid Id) : IRequest<BookingDetailDto>;

public class GetBookingByIdQueryHandler : IRequestHandler<GetBookingByIdQuery, BookingDetailDto>
{
    private readonly IAppDbContext _context;

    public GetBookingByIdQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<BookingDetailDto> Handle(GetBookingByIdQuery request, CancellationToken cancellationToken)
    {
        var booking = await _context.Bookings
            .Include(b => b.Customer)
            .Include(b => b.Venue)
            .Include(b => b.Event)
            .Include(b => b.Payments)
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.Id == request.Id, cancellationToken);

        if (booking == null)
            throw new Exception("Booking not found");

        var paidAmount = booking.Payments.Where(p => p.Status == PaymentStatus.Completed).Sum(p => p.Amount);

        var dto = new BookingDetailDto
        {
            Id = booking.Id,
            ReferenceNumber = booking.ReferenceNumber,
            CustomerId = booking.CustomerId,
            CustomerName = booking.Customer.Name,
            CustomerPhone = booking.Customer.Phone,
            CustomerEmail = booking.Customer.Email ?? "",
            EventId = booking.Event != null ? (Guid?)booking.Event.Id : null,
            EventTitle = booking.Event?.Title,
            VenueId = booking.VenueId,
            Hall = booking.Venue.Name,
            DateStr = booking.BookingDate.ToString("MMM dd, yyyy"),
            Shift = booking.StartTime.Hour < 17 ? "Day" : "Night",
            Guests = booking.GuestCount,
            TotalAmount = booking.TotalAmount,
            PaidAmount = paidAmount,
            Status = booking.Status.ToString(),
            PaymentStatus = GetPaymentStatus(booking.TotalAmount, paidAmount),
            CreatedAt = booking.CreatedAt,
            Payments = booking.Payments.Select(p => new PaymentDto
            {
                Id = p.Id,
                BookingId = booking.Id,
                Amount = p.Amount,
                Method = p.Method.ToString(),
                Status = p.Status.ToString(),
                DateStr = p.PaymentDate.ToString("MMM dd, yyyy HH:mm"),
                Reference = p.ReferenceNumber
            }).ToList()
        };

        return dto;
    }

    private static string GetPaymentStatus(decimal total, decimal paid)
    {
        if (paid >= total) return "Paid";
        if (paid > 0) return "Partial";
        return "Unpaid";
    }
}
