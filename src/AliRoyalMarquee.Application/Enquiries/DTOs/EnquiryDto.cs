using System;
using AliRoyalMarquee.Domain.Enums;

namespace AliRoyalMarquee.Application.Enquiries.DTOs;

public class EnquiryDto
{
    public Guid Id { get; set; }
    public string ReferenceNumber { get; set; } = default!;
    public Guid CustomerId { get; set; }
    public string CustomerName { get; set; } = default!;
    public string CustomerPhone { get; set; } = default!;
    public string EventName { get; set; } = default!;
    public string? EventType { get; set; }
    public DateOnly PreferredDate { get; set; }
    public int GuestCount { get; set; }
    public EnquirySource Source { get; set; }
    public EnquiryPriority Priority { get; set; }
    public EnquiryStatus Status { get; set; }
    public string? AssignedToName { get; set; }
    public decimal? EstimatedValue { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}
