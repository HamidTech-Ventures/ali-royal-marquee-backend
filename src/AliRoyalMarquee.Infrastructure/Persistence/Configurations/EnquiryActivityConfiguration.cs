using AliRoyalMarquee.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AliRoyalMarquee.Infrastructure.Persistence.Configurations;

public class EnquiryActivityConfiguration : IEntityTypeConfiguration<EnquiryActivity>
{
    public void Configure(EntityTypeBuilder<EnquiryActivity> builder)
    {
        builder.HasKey(e => e.Id);
        
        builder.Property(e => e.Description).IsRequired().HasMaxLength(1000);

        builder.HasOne(e => e.PerformedBy)
            .WithMany()
            .HasForeignKey(e => e.PerformedById)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
