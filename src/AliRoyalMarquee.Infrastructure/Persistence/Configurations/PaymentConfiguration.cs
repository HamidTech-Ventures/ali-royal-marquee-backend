using AliRoyalMarquee.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AliRoyalMarquee.Infrastructure.Persistence.Configurations;

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.HasKey(p => p.Id);
        
        builder.Property(p => p.ReferenceNumber).IsRequired().HasMaxLength(50);
        builder.HasIndex(p => p.ReferenceNumber).IsUnique();

        builder.Property(p => p.Amount).HasColumnType("numeric(18,2)");
        
        builder.ToTable(t => t.HasCheckConstraint("CHK_Payment_Amount", "\"Amount\" > 0"));

        builder.HasOne(p => p.Booking)
            .WithMany(b => b.Payments)
            .HasForeignKey(p => p.BookingId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
