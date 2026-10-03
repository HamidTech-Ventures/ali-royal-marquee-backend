using AliRoyalMarquee.Application.Common.Interfaces;
using AliRoyalMarquee.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace AliRoyalMarquee.Infrastructure.Persistence;

public class AppDbContext : DbContext, IAppDbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Venue> Venues => Set<Venue>();
    public DbSet<Enquiry> Enquiries => Set<Enquiry>();
    public DbSet<EnquiryFollowUp> EnquiryFollowUps => Set<EnquiryFollowUp>();
    public DbSet<EnquiryActivity> EnquiryActivities => Set<EnquiryActivity>();
    public DbSet<EnquiryQuotation> EnquiryQuotations => Set<EnquiryQuotation>();
    public DbSet<Booking> Bookings => Set<Booking>();
    public DbSet<Event> Events => Set<Event>();
    public DbSet<EventTask> EventTasks => Set<EventTask>();
    public DbSet<EventStaff> EventStaff => Set<EventStaff>();
    public DbSet<EventMenuItem> EventMenuItems => Set<EventMenuItem>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<Expense> Expenses => Set<Expense>();
    public DbSet<Package> Packages => Set<Package>();
    public DbSet<MenuItem> MenuItems => Set<MenuItem>();
    public DbSet<Addon> Addons => Set<Addon>();
    public DbSet<PricingRule> PricingRules => Set<PricingRule>();
    public DbSet<InventoryItem> InventoryItems => Set<InventoryItem>();
        public DbSet<InventoryMovement> InventoryMovements => Set<InventoryMovement>();
        public DbSet<InventoryReservation> InventoryReservations => Set<InventoryReservation>();
    public DbSet<Vendor> Vendors => Set<Vendor>();
    public DbSet<SystemSetting> SystemSettings => Set<SystemSetting>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<StaffMember> StaffMembers => Set<StaffMember>();
    
    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
    public DbSet<RefreshSession> RefreshSessions => Set<RefreshSession>();
    public DbSet<QuotationLineItem> QuotationLineItems => Set<QuotationLineItem>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("btree_gist");
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
    }

    public Task<Booking?> GetBookingWithPaymentsAndLockAsync(Guid id, CancellationToken cancellationToken)
    {
        return Bookings
            .FromSqlRaw("SELECT * FROM \"Bookings\" WHERE \"Id\" = {0} FOR UPDATE", id)
            .Include(b => b.Payments)
            .FirstOrDefaultAsync(cancellationToken);
    }
}
