using System;
using AliRoyalMarquee.Domain.Common;
using AliRoyalMarquee.Domain.Enums;

namespace AliRoyalMarquee.Domain.Entities;

public class EnquiryFollowUp : BaseEntity
{
    public Guid EnquiryId { get; private set; }
    public Enquiry Enquiry { get; private set; } = default!;

    public DateTime DueDate { get; private set; }
    public TimeSpan? DueTime { get; private set; }
    
    public FollowUpType Type { get; private set; }
    public string? Notes { get; private set; }
    
    public Guid? AssignedToId { get; private set; }
    public User? AssignedTo { get; private set; }

    public FollowUpStatus Status { get; private set; }
    
    public DateTime? CompletedAt { get; private set; }
    public Guid? CompletedById { get; private set; }
    public User? CompletedBy { get; private set; }
    public string? Result { get; private set; }

    private EnquiryFollowUp() { } // EF Core

    public EnquiryFollowUp(Guid enquiryId, DateTime dueDate, TimeSpan? dueTime, FollowUpType type, string? notes, Guid? assignedToId)
    {
        EnquiryId = enquiryId;
        DueDate = dueDate.Date;
        DueTime = dueTime;
        Type = type;
        Notes = notes;
        AssignedToId = assignedToId;
        Status = FollowUpStatus.Pending;
    }

    public void UpdateDetails(DateTime dueDate, TimeSpan? dueTime, FollowUpType type, string? notes, Guid? assignedToId, FollowUpStatus status)
    {
        DueDate = dueDate.Date;
        DueTime = dueTime;
        Type = type;
        Notes = notes;
        AssignedToId = assignedToId;
        Status = status;
    }

    public void Complete(Guid completedById, string? result)
    {
        if (Status == FollowUpStatus.Completed)
            throw new InvalidOperationException("Follow-up is already completed.");

        Status = FollowUpStatus.Completed;
        CompletedAt = DateTime.UtcNow;
        CompletedById = completedById;
        Result = result;
    }

    public void Cancel()
    {
        Status = FollowUpStatus.Cancelled;
    }
}
