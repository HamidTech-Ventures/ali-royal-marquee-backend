using AliRoyalMarquee.Domain.Entities;
using AliRoyalMarquee.Application.Common.Interfaces;
using MediatR;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace AliRoyalMarquee.Application.Bookings.Commands.CreateBooking;

public record CreateBookingCommand(
    Guid CustomerId,
    Guid VenueId,
    DateTime BookingDate,
    DateTime StartTime,
    DateTime EndTime,
    int GuestCount,
    decimal TotalAmount,
    Guid? PackageId = null,
    string? EventTitle = null,
    bool IsDraft = false
) : IRequest<Guid>;

public class CreateBookingValidator : AbstractValidator<CreateBookingCommand>
{
    public CreateBookingValidator()
    {
        RuleFor(v => v.CustomerId).NotEmpty();
        RuleFor(v => v.VenueId).NotEmpty();
        RuleFor(v => v.StartTime).LessThan(v => v.EndTime).WithMessage("StartTime must be before EndTime.");
        RuleFor(v => v.GuestCount).GreaterThan(0);
        RuleFor(v => v.TotalAmount).GreaterThanOrEqualTo(0);
    }
}

public class CreateBookingCommandHandler : IRequestHandler<CreateBookingCommand, Guid>
{
    private readonly IAppDbContext _context;

    public CreateBookingCommandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(CreateBookingCommand request, CancellationToken cancellationToken)
    {
        var overlapExists = await _context.Bookings
            .AnyAsync(b => b.VenueId == request.VenueId 
                        && b.StartTime < request.EndTime 
                        && b.EndTime > request.StartTime 
                        && b.Status != Domain.Enums.BookingStatus.Cancelled, 
                        cancellationToken);

        if (overlapExists)
            throw new InvalidOperationException("The venue is already booked for the specified time range.");

        var currentMonth = DateTime.UtcNow.ToString("yyyyMM");
        var prefix = $"BKG-{currentMonth}-";
        
        var lastBooking = await _context.Bookings
            .Where(b => b.ReferenceNumber.StartsWith(prefix))
            .OrderByDescending(b => b.ReferenceNumber)
            .FirstOrDefaultAsync(cancellationToken);
            
        int nextNumber = 1;
        if (lastBooking != null)
        {
            var lastNumberStr = lastBooking.ReferenceNumber.Replace(prefix, "");
            if (int.TryParse(lastNumberStr, out int lastNumber))
            {
                nextNumber = lastNumber + 1;
            }
        }
        
        var referenceNumber = $"{prefix}{nextNumber.ToString("D4")}";

        var booking = new Booking(
            request.CustomerId,
            request.VenueId,
            referenceNumber,
            request.BookingDate,
            request.StartTime,
            request.EndTime,
            request.GuestCount,
            request.TotalAmount,
            request.PackageId
        );

        if (request.IsDraft) 
        {
            booking.MarkAsDraft();
        }

        _context.Bookings.Add(booking);
        
        if (!string.IsNullOrWhiteSpace(request.EventTitle))
        {
            var eventRef = referenceNumber.Replace("BKG-", "EVT-");
            var newEvent = new Event(booking.Id, eventRef, request.EventTitle, null);
            _context.Events.Add(newEvent);
        }

        await _context.SaveChangesAsync(cancellationToken);

        return booking.Id;
    }
}
