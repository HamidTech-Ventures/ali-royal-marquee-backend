using System;
using AliRoyalMarquee.Domain.Common;

namespace AliRoyalMarquee.Domain.Entities
{
    public class InventoryMovement : BaseEntity
    {
        public Guid InventoryItemId { get; set; }
        public InventoryItem InventoryItem { get; set; }
        public string Type { get; set; } // IN, OUT, RELOCATE
        public decimal Quantity { get; set; }
        public string Notes { get; set; }
        public string Reference { get; set; } // e.g. PO-123 or Event-456
    }
}
