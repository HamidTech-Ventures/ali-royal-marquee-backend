using System;
using AliRoyalMarquee.Domain.Common;

namespace AliRoyalMarquee.Domain.Entities
{
    public class Package : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Status { get; set; } = "Active";
        public int MinGuests { get; set; }
        
        public string? InternalNotes { get; set; }
        public string? InclusionsJson { get; set; }
    }
}
