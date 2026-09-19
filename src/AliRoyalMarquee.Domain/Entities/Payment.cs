using System;
using AliRoyalMarquee.Domain.Common;
using AliRoyalMarquee.Domain.Enums;

namespace AliRoyalMarquee.Domain.Entities;

public class Payment : BaseEntity
{
    public string ReferenceNumber { get; private set; } = default!;
    
    public Guid BookingId { get; private set; }
    public Booking Booking { get; private set; } = default!;

    public Guid CustomerId { get; private set; }
    
    public decimal Amount { get; private set; }
    public PaymentMethod Method { get; private set; }
    public PaymentStatus Status { get; private set; }
    public DateTime PaymentDate { get; private set; }

    public PaymentType Type { get; private set; }
    public Guid? OriginalPaymentId { get; private set; }

    private Payment() { }

    public Payment(Guid bookingId, Guid customerId, string referenceNumber, decimal amount, PaymentMethod method, DateTime paymentDate, PaymentType type = PaymentType.Payment, Guid? originalPaymentId = null)
    {
        if (amount <= 0)
            throw new ArgumentException("Amount must be greater than zero.");

        BookingId = bookingId;
        CustomerId = customerId;
        ReferenceNumber = referenceNumber;
        Amount = amount;
        Method = method;
        PaymentDate = paymentDate;
        Type = type;
        OriginalPaymentId = originalPaymentId;
        Status = PaymentStatus.Completed;
    }
}
