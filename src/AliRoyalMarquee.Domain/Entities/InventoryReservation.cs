using System;
using AliRoyalMarquee.Domain.Common;

namespace AliRoyalMarquee.Domain.Entities
{
    public class InventoryReservation : BaseEntity
    {
        public Guid InventoryItemId { get; set; }
        public InventoryItem InventoryItem { get; set; }
        public Guid EventId { get; set; }
        public Event Event { get; set; }
        public decimal Quantity { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; } // Active, Completed, Cancelled
    }
}
