using System;

namespace AliRoyalMarquee.Application.Inventory.DTOs
{
    public class InventoryItemDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public decimal Quantity { get; set; }
        public decimal MinQuantity { get; set; }
        public string Unit { get; set; }
        public string ItemType { get; set; }
        public decimal UnitPrice { get; set; }
        public string Location { get; set; }
        public System.Collections.Generic.List<InventoryMovementDto> Movements { get; set; } = new();
        public System.Collections.Generic.List<InventoryReservationDto> Reservations { get; set; } = new();
        
        // Dynamically computed
        public string Status => Quantity == 0 ? "Out of Stock" : (Quantity <= MinQuantity ? "Low Stock" : "In Stock");
    }
}
