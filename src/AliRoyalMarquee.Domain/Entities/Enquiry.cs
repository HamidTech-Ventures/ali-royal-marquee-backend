using System;
using AliRoyalMarquee.Domain.Common;
using AliRoyalMarquee.Domain.Enums;

namespace AliRoyalMarquee.Domain.Entities;

public class Enquiry : BaseEntity
{
    public string ReferenceNumber { get; private set; } = default!;
    
    public Guid CustomerId { get; private set; }
    public Customer Customer { get; private set; } = default!;

    public string EventName { get; private set; } = default!;
    public string? EventType { get; private set; }
    
    public DateOnly PreferredDate { get; private set; }
    public DateOnly? AlternativeDate { get; private set; }
    public EventShift Shift { get; private set; }
    public int GuestCount { get; private set; }
    public int BufferCapacity { get; private set; }
    public bool PartitionRequired { get; private set; }
    
    public Guid? PreferredVenueId { get; private set; }
    public Venue? PreferredVenue { get; private set; }
    
    public decimal? Budget { get; private set; }
    public EnquirySource Source { get; private set; }
    public Guid? AssignedToId { get; private set; }
    public User? AssignedTo { get; private set; }
    
    public string? Notes { get; private set; }
    public decimal? EstimatedValue { get; private set; }
    public EnquiryStatus Status { get; private set; }
    public string? LostReason { get; private set; }

    public IReadOnlyCollection<EnquiryFollowUp> FollowUps => _followUps.AsReadOnly();
    private readonly List<EnquiryFollowUp> _followUps = new();

    public IReadOnlyCollection<EnquiryActivity> Activities => _activities.AsReadOnly();
    private readonly List<EnquiryActivity> _activities = new();

    public IReadOnlyCollection<EnquiryQuotation> Quotations => _quotations.AsReadOnly();
    private readonly List<EnquiryQuotation> _quotations = new();

    private Enquiry() { } // EF Core

    public Enquiry(
        Guid customerId, 
        string referenceNumber, 
        string eventName, 
        string? eventType, 
        DateOnly preferredDate, 
        int guestCount,
        EnquirySource source)
    {
        CustomerId = customerId;
        ReferenceNumber = referenceNumber;
        EventName = eventName;
        EventType = eventType;
        PreferredDate = preferredDate;
        GuestCount = guestCount;
        Source = source;
        Status = EnquiryStatus.Inquiry;
    }

    public void UpdateDetails(
        string eventName, 
        string? eventType, 
        DateOnly preferredDate, 
        DateOnly? alternativeDate,
        EventShift shift,
        int guestCount,
        int bufferCapacity,
        bool partitionRequired,
        Guid? preferredVenueId,
        decimal? budget,
        EnquirySource source,
        Guid? assignedToId,
        string? notes,
        decimal? estimatedValue)
    {
        EventName = eventName;
        EventType = eventType;
        PreferredDate = preferredDate;
        AlternativeDate = alternativeDate;
        Shift = shift;
        GuestCount = guestCount;
        BufferCapacity = bufferCapacity;
        PartitionRequired = partitionRequired;
        PreferredVenueId = preferredVenueId;
        Budget = budget;
        Source = source;
        AssignedToId = assignedToId;
        Notes = notes;
        EstimatedValue = estimatedValue;
    }

    public void UpdateStatus(EnquiryStatus newStatus)
    {
        Status = newStatus;
        if (newStatus != EnquiryStatus.Cancelled)
        {
            LostReason = null;
        }
    }

    public void MarkLost(string reason)
    {
        if (string.IsNullOrWhiteSpace(reason))
            throw new ArgumentException("Reason is required when marking enquiry as lost.");
        
        Status = EnquiryStatus.Cancelled;
        LostReason = reason;
    }

    public void ConvertToBooking()
    {
        if (Status == EnquiryStatus.AdvancePaid)
            throw new InvalidOperationException("Enquiry is already converted.");
            
        Status = EnquiryStatus.AdvancePaid;
    }
}
