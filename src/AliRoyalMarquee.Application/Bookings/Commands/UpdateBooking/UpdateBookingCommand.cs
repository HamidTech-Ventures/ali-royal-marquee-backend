using AliRoyalMarquee.Application.Common.Interfaces;
using MediatR;
using FluentValidation;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace AliRoyalMarquee.Application.Bookings.Commands.UpdateBooking;

public record UpdateBookingCommand(
    Guid Id,
    Guid VenueId,
    DateTime BookingDate,
    DateTime StartTime,
    DateTime EndTime,
    int GuestCount,
    decimal TotalAmount
) : IRequest;

public class UpdateBookingValidator : AbstractValidator<UpdateBookingCommand>
{
    public UpdateBookingValidator()
    {
        RuleFor(v => v.Id).NotEmpty();
        RuleFor(v => v.VenueId).NotEmpty();
        RuleFor(v => v.StartTime).LessThan(v => v.EndTime).WithMessage("StartTime must be before EndTime.");
        RuleFor(v => v.GuestCount).GreaterThan(0);
        RuleFor(v => v.TotalAmount).GreaterThanOrEqualTo(0);
    }
}

public class UpdateBookingCommandHandler : IRequestHandler<UpdateBookingCommand>
{
    private readonly IAppDbContext _context;

    public UpdateBookingCommandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task Handle(UpdateBookingCommand request, CancellationToken cancellationToken)
    {
        var booking = await _context.Bookings.FindAsync(new object[] { request.Id }, cancellationToken);

        if (booking == null)
            throw new Exception("Booking not found");

        var overlapExists = await _context.Bookings
            .AnyAsync(b => b.Id != request.Id 
                        && b.VenueId == request.VenueId 
                        && b.StartTime < request.EndTime 
                        && b.EndTime > request.StartTime 
                        && b.Status != Domain.Enums.BookingStatus.Cancelled, 
                        cancellationToken);

        if (overlapExists)
            throw new InvalidOperationException("The venue is already booked for the specified time range.");

        booking.Update(
            request.VenueId,
            request.BookingDate,
            request.StartTime,
            request.EndTime,
            request.GuestCount,
            request.TotalAmount
        );

        await _context.SaveChangesAsync(cancellationToken);
    }
}

public record UpdateBookingPackageCommand(Guid BookingId, Guid? PackageId) : IRequest;

public class UpdateBookingPackageCommandHandler : IRequestHandler<UpdateBookingPackageCommand>
{
    private readonly IAppDbContext _context;
    public UpdateBookingPackageCommandHandler(IAppDbContext context) => _context = context;

    public async Task Handle(UpdateBookingPackageCommand request, CancellationToken cancellationToken)
    {
        var booking = await _context.Bookings.FindAsync(new object[] { request.BookingId }, cancellationToken);
        if (booking == null) throw new Exception("Booking not found");

        booking.UpdatePackage(request.PackageId);

        decimal packagePrice = 0;

        if (request.PackageId.HasValue)
        {
            var package = await _context.Packages.FindAsync(new object[] { request.PackageId.Value }, cancellationToken);
            if (package != null)
            {
                if (!string.IsNullOrEmpty(package.Type) && package.Type.Contains("Fixed", StringComparison.OrdinalIgnoreCase))
                {
                    packagePrice = package.Price;
                }
                else
                {
                    packagePrice = package.Price * booking.GuestCount;
                }
            }
        }

        booking.SetTotalAmount(packagePrice);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
