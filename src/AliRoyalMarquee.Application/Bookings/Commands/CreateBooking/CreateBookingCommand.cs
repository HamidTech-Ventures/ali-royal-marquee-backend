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
    Guid? PackageId = null
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
        var bookingCount = await _context.Bookings
            .CountAsync(b => b.ReferenceNumber.StartsWith($"BKG-{currentMonth}-"), cancellationToken);
        
        var referenceNumber = $"BKG-{currentMonth}-{(bookingCount + 1).ToString("D4")}";

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

        _context.Bookings.Add(booking);
        
        await _context.SaveChangesAsync(cancellationToken);

        return booking.Id;
    }
}
