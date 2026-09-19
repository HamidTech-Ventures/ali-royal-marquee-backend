using System;
using AliRoyalMarquee.Domain.Common;

namespace AliRoyalMarquee.Domain.Entities
{
    public class StaffMember : BaseEntity
    {
        public string Name { get; set; }
        public string Role { get; set; }
        public string Phone { get; set; }
        public string Shift { get; set; }
        public string Status { get; set; }
        public decimal? Salary { get; set; }
    }
}
