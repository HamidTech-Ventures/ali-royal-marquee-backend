using AliRoyalMarquee.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AliRoyalMarquee.Infrastructure.Persistence.Configurations;

public class EnquiryFollowUpConfiguration : IEntityTypeConfiguration<EnquiryFollowUp>
{
    public void Configure(EntityTypeBuilder<EnquiryFollowUp> builder)
    {
        builder.HasKey(e => e.Id);
        
        builder.Property(e => e.Notes).HasMaxLength(1000);
        builder.Property(e => e.Result).HasMaxLength(1000);

        builder.HasOne(e => e.AssignedTo)
            .WithMany()
            .HasForeignKey(e => e.AssignedToId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(e => e.CompletedBy)
            .WithMany()
            .HasForeignKey(e => e.CompletedById)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
