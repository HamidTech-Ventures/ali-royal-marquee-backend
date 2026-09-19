using AliRoyalMarquee.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AliRoyalMarquee.Application.Common.Interfaces;

public interface IAppDbContext
{
    DbSet<Customer> Customers { get; }
    DbSet<Venue> Venues { get; }
    DbSet<Enquiry> Enquiries { get; }
    DbSet<EnquiryFollowUp> EnquiryFollowUps { get; }
    DbSet<EnquiryActivity> EnquiryActivities { get; }
    DbSet<EnquiryQuotation> EnquiryQuotations { get; }
    DbSet<Booking> Bookings { get; }
    DbSet<Event> Events { get; }
    DbSet<EventTask> EventTasks { get; }
    DbSet<EventStaff> EventStaff { get; }
    DbSet<EventMenuItem> EventMenuItems { get; }
    DbSet<Payment> Payments { get; }
    DbSet<Expense> Expenses { get; }
    DbSet<Package> Packages { get; }
    DbSet<MenuItem> MenuItems { get; }
    DbSet<Addon> Addons { get; }
    DbSet<PricingRule> PricingRules { get; }
    DbSet<InventoryItem> InventoryItems { get; }
    DbSet<Vendor> Vendors { get; }
    DbSet<StaffMember> StaffMembers { get; }

    DbSet<User> Users { get; }
    DbSet<Role> Roles { get; }
    DbSet<Permission> Permissions { get; }
    DbSet<RolePermission> RolePermissions { get; }
    DbSet<RefreshSession> RefreshSessions { get; }
    DbSet<QuotationLineItem> QuotationLineItems { get; }
    DbSet<AuditLog> AuditLogs { get; }

    Microsoft.EntityFrameworkCore.Infrastructure.DatabaseFacade Database { get; }

    Task<Booking?> GetBookingWithPaymentsAndLockAsync(Guid id, CancellationToken cancellationToken);

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
