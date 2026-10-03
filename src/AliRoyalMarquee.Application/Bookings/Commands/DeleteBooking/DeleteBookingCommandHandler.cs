using AliRoyalMarquee.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AliRoyalMarquee.Application.Bookings.Commands.DeleteBooking;

public class DeleteBookingCommandHandler : IRequestHandler<DeleteBookingCommand>
{
    private readonly IAppDbContext _context;

    public DeleteBookingCommandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task Handle(DeleteBookingCommand request, CancellationToken cancellationToken)
    {
        var booking = await _context.Bookings
            .FirstOrDefaultAsync(b => b.Id == request.Id, cancellationToken);

        if (booking == null)
        {
            throw new Exception($"Booking {request.Id} not found");
        }

        _context.Bookings.Remove(booking);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
