using AliRoyalMarquee.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AliRoyalMarquee.Infrastructure.Persistence.Configurations;

public class BookingConfiguration : IEntityTypeConfiguration<Booking>
{
    public void Configure(EntityTypeBuilder<Booking> builder)
    {
        builder.HasKey(b => b.Id);
        
        builder.Property(b => b.ReferenceNumber).IsRequired().HasMaxLength(50);
        builder.HasIndex(b => b.ReferenceNumber).IsUnique();
        
        builder.HasIndex(b => b.BookingDate);
        builder.HasIndex(b => b.StartTime);

        builder.Property(b => b.TotalAmount).HasColumnType("numeric(18,2)");

        builder.HasOne(b => b.Customer)
            .WithMany(c => c.Bookings)
            .HasForeignKey(b => b.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(b => b.Venue)
            .WithMany()
            .HasForeignKey(b => b.VenueId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.ToTable(t => t.HasCheckConstraint("CHK_Booking_Dates", "\"StartTime\" < \"EndTime\""));
    }
}
