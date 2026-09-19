using AliRoyalMarquee.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AliRoyalMarquee.Infrastructure.Persistence.Configurations;

public class EventConfiguration : IEntityTypeConfiguration<Event>
{
    public void Configure(EntityTypeBuilder<Event> builder)
    {
        builder.HasKey(e => e.Id);
        
        builder.Property(e => e.ReferenceNumber).IsRequired().HasMaxLength(50);
        builder.HasIndex(e => e.ReferenceNumber).IsUnique();
        
        builder.Property(e => e.Title).IsRequired().HasMaxLength(200);
        builder.Property(e => e.ManagerId).HasMaxLength(100);

        builder.HasOne(e => e.Booking)
            .WithOne(b => b.Event)
            .HasForeignKey<Event>(e => e.BookingId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
