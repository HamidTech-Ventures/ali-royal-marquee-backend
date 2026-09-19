using AliRoyalMarquee.Domain.Entities;
using AliRoyalMarquee.Domain.Enums;
using AliRoyalMarquee.Application.Common.Interfaces;
using MediatR;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace AliRoyalMarquee.Application.Payments.Commands.RecordPayment;

public record RecordPaymentCommand(
    Guid BookingId,
    decimal Amount,
    PaymentMethod Method,
    DateTime PaymentDate,
    string? Reference,
    PaymentType Type = PaymentType.Payment,
    Guid? OriginalPaymentId = null
) : IRequest<Guid>;

public class RecordPaymentValidator : AbstractValidator<RecordPaymentCommand>
{
    public RecordPaymentValidator()
    {
        RuleFor(v => v.BookingId).NotEmpty();
        RuleFor(v => v.Amount).GreaterThan(0);
        RuleFor(v => v.PaymentDate).NotEmpty();
    }
}

public class RecordPaymentCommandHandler : IRequestHandler<RecordPaymentCommand, Guid>
{
    private readonly IAppDbContext _context;

    public RecordPaymentCommandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(RecordPaymentCommand request, CancellationToken cancellationToken)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

        // Lock the booking row for update to prevent concurrent overpayments
        var booking = await _context.GetBookingWithPaymentsAndLockAsync(request.BookingId, cancellationToken);

        if (booking == null)
            throw new InvalidOperationException("Booking not found.");

        // Calculate current payments and refunds
        var totalPaid = booking.Payments.Where(p => p.Type == PaymentType.Payment).Sum(p => p.Amount);
        var totalRefunded = booking.Payments.Where(p => p.Type == PaymentType.Refund).Sum(p => p.Amount);
        var netCollected = totalPaid - totalRefunded;

        var remainingAmount = booking.TotalAmount - netCollected;

        if (request.Type == PaymentType.Payment && request.Amount > remainingAmount)
        {
            throw new ValidationException($"Payment amount {request.Amount} exceeds the remaining balance {remainingAmount}.");
        }
        
        if (request.Type == PaymentType.Refund && request.Amount > netCollected)
        {
            throw new ValidationException($"Refund amount {request.Amount} exceeds the net collected amount {netCollected}.");
        }

        var referenceNumber = request.Reference ?? $"{(request.Type == PaymentType.Payment ? "PAY" : "REF")}-{DateTime.UtcNow.Ticks.ToString().Substring(10)}";

        var payment = new Payment(
            booking.Id,
            booking.CustomerId,
            referenceNumber,
            request.Amount,
            request.Method,
            request.PaymentDate,
            request.Type,
            request.OriginalPaymentId
        );

        _context.Payments.Add(payment);
        await _context.SaveChangesAsync(cancellationToken);
        
        await transaction.CommitAsync(cancellationToken);

        return payment.Id;
    }
}
