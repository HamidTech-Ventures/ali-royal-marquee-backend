using System;
using AliRoyalMarquee.Domain.Common;
using AliRoyalMarquee.Domain.Enums;

namespace AliRoyalMarquee.Domain.Entities;

public class EnquiryActivity : BaseEntity
{
    public Guid EnquiryId { get; private set; }
    public Enquiry Enquiry { get; private set; } = default!;

    public EnquiryActivityType Type { get; private set; }
    public string Description { get; private set; } = default!;
    
    public Guid PerformedById { get; private set; }
    public User PerformedBy { get; private set; } = default!;
    
    public DateTime Timestamp { get; private set; }

    private EnquiryActivity() { } // EF Core

    public EnquiryActivity(Guid enquiryId, EnquiryActivityType type, string description, Guid performedById)
    {
        EnquiryId = enquiryId;
        Type = type;
        Description = description;
        PerformedById = performedById;
        Timestamp = DateTime.UtcNow;
    }
}
