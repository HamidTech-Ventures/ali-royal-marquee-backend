using System;
using System.Collections.Generic;

namespace AliRoyalMarquee.Application.Bookings.DTOs;

public class PaymentDto
{
    public Guid Id { get; set; }
    public Guid BookingId { get; set; }
    public decimal Amount { get; set; }
    public string Method { get; set; } = default!;
    public string Status { get; set; } = default!;
    public string DateStr { get; set; } = default!;
    public string Reference { get; set; } = default!;
}

public class BookingDetailDto
{
    public Guid Id { get; set; }
    public string ReferenceNumber { get; set; } = default!;
    public Guid CustomerId { get; set; }
    public string CustomerName { get; set; } = default!;
    public string CustomerPhone { get; set; } = default!;
    public string CustomerEmail { get; set; } = default!;
    public Guid? EventId { get; set; }
    public string? EventTitle { get; set; }
    public Guid VenueId { get; set; }
    public string Hall { get; set; } = default!;
    public string DateStr { get; set; } = default!;
    public string Shift { get; set; } = default!;
    public int Guests { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public string Status { get; set; } = default!;
    public string PaymentStatus { get; set; } = default!;
    public DateTimeOffset CreatedAt { get; set; }
    
    public List<PaymentDto> Payments { get; set; } = new();
}
