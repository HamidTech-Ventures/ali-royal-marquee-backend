using System;

namespace AliRoyalMarquee.Application.StaffMembers.DTOs
{
    public class StaffDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Role { get; set; }
        public string Phone { get; set; }
        public string Shift { get; set; }
        public string Status { get; set; }
        public decimal? Salary { get; set; }
    }
}
