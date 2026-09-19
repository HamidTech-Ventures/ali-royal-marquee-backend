using AliRoyalMarquee.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AliRoyalMarquee.Infrastructure.Persistence.Configurations;

public class EnquiryConfiguration : IEntityTypeConfiguration<Enquiry>
{
    public void Configure(EntityTypeBuilder<Enquiry> builder)
    {
        builder.HasKey(e => e.Id);
        
        builder.Property(e => e.ReferenceNumber).IsRequired().HasMaxLength(50);
        builder.HasIndex(e => e.ReferenceNumber).IsUnique();
        
        builder.Property(e => e.EventName).IsRequired().HasMaxLength(100);
        builder.Property(e => e.EventType).HasMaxLength(100);

        builder.HasOne(e => e.Customer)
            .WithMany(c => c.Enquiries)
            .HasForeignKey(e => e.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.PreferredVenue)
            .WithMany()
            .HasForeignKey(e => e.PreferredVenueId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(e => e.AssignedTo)
            .WithMany()
            .HasForeignKey(e => e.AssignedToId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Property(e => e.Budget).HasColumnType("decimal(18,2)");
        builder.Property(e => e.EstimatedValue).HasColumnType("decimal(18,2)");

        builder.HasMany(e => e.FollowUps)
            .WithOne(f => f.Enquiry)
            .HasForeignKey(f => f.EnquiryId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(e => e.Activities)
            .WithOne(a => a.Enquiry)
            .HasForeignKey(a => a.EnquiryId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(e => e.Quotations)
            .WithOne(q => q.Enquiry)
            .HasForeignKey(q => q.EnquiryId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
