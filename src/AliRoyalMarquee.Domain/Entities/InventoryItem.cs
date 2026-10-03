using System;
using AliRoyalMarquee.Domain.Common;

namespace AliRoyalMarquee.Domain.Entities
{
    public class InventoryItem : BaseEntity
    {
        public string Name { get; set; }
        public string Category { get; set; }
        public decimal Quantity { get; set; }
        public decimal MinQuantity { get; set; }
        public string Unit { get; set; }
        public string ItemType { get; set; } = "Fixed Asset";
        public decimal UnitPrice { get; set; }
        public string Location { get; set; }
        public ICollection<InventoryMovement> Movements { get; set; } = new List<InventoryMovement>();
        public ICollection<InventoryReservation> Reservations { get; set; } = new List<InventoryReservation>();
    }
}
