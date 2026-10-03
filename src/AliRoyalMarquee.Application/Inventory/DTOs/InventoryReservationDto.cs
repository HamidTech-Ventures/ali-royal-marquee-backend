using System;
namespace AliRoyalMarquee.Application.Inventory.DTOs
{
    public class InventoryReservationDto
    {
        public Guid Id { get; set; }
        public Guid EventId { get; set; }
        public decimal Quantity { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
    }
}
