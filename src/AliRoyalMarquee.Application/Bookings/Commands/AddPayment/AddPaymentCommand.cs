using AliRoyalMarquee.Application.Common.Interfaces;
using AliRoyalMarquee.Domain.Entities;
using AliRoyalMarquee.Domain.Enums;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AliRoyalMarquee.Application.Bookings.Commands.AddPayment;

public record AddPaymentCommand(
    Guid BookingId,
    decimal Amount,
    string Method,
    string ReferenceNumber,
    Guid UserId
) : IRequest<Guid>;

public class AddPaymentCommandHandler : IRequestHandler<AddPaymentCommand, Guid>
{
    private readonly IAppDbContext _context;

    public AddPaymentCommandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(AddPaymentCommand request, CancellationToken cancellationToken)
    {
        var booking = await _context.Bookings.FindAsync(new object[] { request.BookingId }, cancellationToken);
        if (booking == null)
            throw new Exception("Booking not found");

        if (!Enum.TryParse<PaymentMethod>(request.Method, true, out var methodEnum))
            throw new Exception("Invalid payment method");

        var payment = new Payment(
            booking.Id,
            booking.CustomerId,
            request.ReferenceNumber,
            request.Amount,
            methodEnum,
            DateTime.UtcNow
        );

        // We can just add to the Payments DbSet if it exists, or via booking.AddPayment
        booking.AddPayment(payment);
        if (booking.Status == BookingStatus.Pending) {
            booking.Confirm();
        }
        
        await _context.SaveChangesAsync(cancellationToken);
        
        return payment.Id;
    }
}
