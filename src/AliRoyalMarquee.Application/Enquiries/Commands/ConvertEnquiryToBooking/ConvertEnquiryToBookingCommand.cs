using AliRoyalMarquee.Application.Common.Interfaces;
using AliRoyalMarquee.Domain.Entities;
using AliRoyalMarquee.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AliRoyalMarquee.Application.Enquiries.Commands.ConvertEnquiryToBooking;

public record ConvertEnquiryToBookingCommand(
    Guid Id,
    Guid VenueId,
    DateTime Date,
    DateTime StartTime,
    DateTime EndTime,
    int GuestCount,
    decimal TotalAmount,
    Guid PerformedById,
    Guid? PackageId = null) : IRequest<Guid>;

public class ConvertEnquiryToBookingCommandHandler : IRequestHandler<ConvertEnquiryToBookingCommand, Guid>
{
    private readonly IAppDbContext _context;

    public ConvertEnquiryToBookingCommandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(ConvertEnquiryToBookingCommand request, CancellationToken cancellationToken)
    {
        // 1. Transaction is handled via EF Core's SaveChangesAsync implicitly, or we can use explicit transaction.
        // We'll rely on the underlying PostgreSQL transaction from EF Core.
        using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            var enquiry = await _context.Enquiries
                .FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken);

            if (enquiry == null) throw new Exception("Enquiry not found");
            
            if (enquiry.Status == EnquiryStatus.AdvancePaid)
                throw new Exception("Enquiry is already converted to a booking.");

            // 2. Check venue availability
            // Ensure no overlapping active/tentative booking exists for the venue in the requested time frame.
            var hasConflict = await _context.Bookings
                .Where(b => b.VenueId == request.VenueId)
                .Where(b => b.Status != BookingStatus.Cancelled) // Only active/tentative block slots
                .Where(b => b.BookingDate == request.Date.Date)
                .Where(b => request.StartTime < b.EndTime && request.EndTime > b.StartTime)
                .AnyAsync(cancellationToken);

            if (hasConflict)
            {
                throw new Exception("The selected venue is not available for the requested time range. Please choose a different venue or time.");
            }

            // 3. Create Booking
            var refNumber = $"BKG-{DateTime.UtcNow.Ticks.ToString().Substring(10)}";
            var booking = new Booking(
                enquiry.CustomerId,
                request.VenueId,
                refNumber,
                request.Date,
                request.StartTime,
                request.EndTime,
                request.GuestCount,
                request.TotalAmount,
                request.PackageId
            );

            _context.Bookings.Add(booking);

            // 4. Mark Enquiry Converted
            enquiry.ConvertToBooking();

            // 5. Create Activity
            var activity = new EnquiryActivity(
                enquiry.Id,
                EnquiryActivityType.Converted,
                $"Enquiry converted to Booking {refNumber}",
                request.PerformedById
            );
            _context.EnquiryActivities.Add(activity);

            await _context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return booking.Id;
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
}
