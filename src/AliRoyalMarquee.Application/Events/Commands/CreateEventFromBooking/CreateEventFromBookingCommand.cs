using AliRoyalMarquee.Application.Common.Interfaces;
using AliRoyalMarquee.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AliRoyalMarquee.Application.Events.Commands.CreateEventFromBooking;

public record CreateEventFromBookingCommand(Guid BookingId) : IRequest<Guid>;

public class CreateEventFromBookingCommandHandler : IRequestHandler<CreateEventFromBookingCommand, Guid>
{
    private readonly IAppDbContext _context;
    public CreateEventFromBookingCommandHandler(IAppDbContext context) { _context = context; }

    public async Task<Guid> Handle(CreateEventFromBookingCommand request, CancellationToken cancellationToken)
    {
        var booking = await _context.Bookings.FindAsync(new object[] { request.BookingId }, cancellationToken);
        if (booking == null) throw new Exception("Booking not found");

        var existingEvent = await _context.Events.FirstOrDefaultAsync(e => e.BookingId == request.BookingId, cancellationToken);
        if (existingEvent != null) return existingEvent.Id;

        var eventRef = booking.ReferenceNumber.Replace("BKG-", "EVT-");
        var title = booking.Event?.Title ?? $"Event for {booking.ReferenceNumber}";

        var newEvent = new Event(booking.Id, eventRef, title, null);
        
        _context.Events.Add(newEvent);
        
        // Update booking status if you want, e.g., booking.Complete();
        await _context.SaveChangesAsync(cancellationToken);
        
        return newEvent.Id;
    }
}
