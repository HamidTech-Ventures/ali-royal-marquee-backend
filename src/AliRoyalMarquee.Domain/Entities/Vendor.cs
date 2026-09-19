using System;
using AliRoyalMarquee.Domain.Common;

namespace AliRoyalMarquee.Domain.Entities
{
    public class Vendor : BaseEntity
    {
        public string Name { get; set; }
        public string Category { get; set; }
        public string ContactName { get; set; }
        public string Phone { get; set; }
        public string Status { get; set; }
    }
}
