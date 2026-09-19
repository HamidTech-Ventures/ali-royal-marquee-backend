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
    }
}
