using System;

namespace AliRoyalMarquee.Application.Vendors.DTOs
{
    public class VendorDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public string ContactName { get; set; }
        public string Phone { get; set; }
        public string Status { get; set; }
    }
}
