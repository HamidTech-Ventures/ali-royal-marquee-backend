using System;
using System.Collections.Generic;
using AliRoyalMarquee.Domain.Enums;

namespace AliRoyalMarquee.Application.Enquiries.DTOs;

public class EnquiryDetailDto : EnquiryDto
{
    public DateOnly? AlternativeDate { get; set; }

    public Guid? PreferredVenueId { get; set; }
    public string? PreferredVenueName { get; set; }
    public decimal? Budget { get; set; }
    public Guid? AssignedToId { get; set; }
    public string? Notes { get; set; }
    public string? LostReason { get; set; }

    public List<EnquiryFollowUpDto> FollowUps { get; set; } = new();
    public List<EnquiryQuotationDto> Quotations { get; set; } = new();
    public List<EnquiryActivityDto> Activities { get; set; } = new();
}

public class EnquiryFollowUpDto
{
    public Guid Id { get; set; }
    public DateTime DueDate { get; set; }
    public TimeSpan? DueTime { get; set; }
    public FollowUpType Type { get; set; }
    public string? Notes { get; set; }
    public Guid? AssignedToId { get; set; }
    public string? AssignedToName { get; set; }
    public FollowUpStatus Status { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string? CompletedByName { get; set; }
    public string? Result { get; set; }
}

public class EnquiryQuotationDto
{
    public Guid Id { get; set; }
    public string QuotationReference { get; set; } = default!;
    public int Version { get; set; }
    public decimal Amount { get; set; }
    public DateTime? ValidUntil { get; set; }
    public QuotationStatus Status { get; set; }
    public string? Notes { get; set; }
    public string CreatorName { get; set; } = default!;
    public DateTimeOffset CreatedAt { get; set; }
}

public class EnquiryActivityDto
{
    public Guid Id { get; set; }
    public EnquiryActivityType Type { get; set; }
    public string Description { get; set; } = default!;
    public string PerformedByName { get; set; } = default!;
    public DateTime Timestamp { get; set; }
}
