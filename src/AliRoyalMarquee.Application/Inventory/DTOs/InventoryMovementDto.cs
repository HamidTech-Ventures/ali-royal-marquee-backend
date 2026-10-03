using System;
namespace AliRoyalMarquee.Application.Inventory.DTOs
{
    public class InventoryMovementDto
    {
        public Guid Id { get; set; }
        public string Type { get; set; }
        public decimal Quantity { get; set; }
        public string Notes { get; set; }
        public string Reference { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
    }
}
