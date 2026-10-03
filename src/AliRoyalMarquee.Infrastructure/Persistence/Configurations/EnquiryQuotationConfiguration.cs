using AliRoyalMarquee.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AliRoyalMarquee.Infrastructure.Persistence.Configurations;

public class EnquiryQuotationConfiguration : IEntityTypeConfiguration<EnquiryQuotation>
{
    public void Configure(EntityTypeBuilder<EnquiryQuotation> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.QuotationReference)
            .IsRequired()
            .HasMaxLength(50);

        // Financial precision
        builder.Property(x => x.Subtotal).HasPrecision(18, 2);
        builder.Property(x => x.DiscountAmount).HasPrecision(18, 2);
        builder.Property(x => x.ServiceChargeAmount).HasPrecision(18, 2);
        builder.Property(x => x.PRATaxAmount).HasPrecision(18, 2);
        builder.Property(x => x.TokenMoney).HasPrecision(18, 2);
        builder.Property(x => x.AdvancePayment).HasPrecision(18, 2);
        builder.Property(x => x.GrandTotal).HasPrecision(18, 2);

        builder.Property(x => x.Notes)
            .HasMaxLength(1000);

        builder.HasOne(x => x.Enquiry)
            .WithMany(e => e.Quotations)
            .HasForeignKey(x => x.EnquiryId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Creator)
            .WithMany()
            .HasForeignKey(x => x.CreatorId)
            .OnDelete(DeleteBehavior.Restrict);

        // Self-referencing relationship for versions
        builder.HasOne(x => x.PreviousQuotation)
            .WithMany()
            .HasForeignKey(x => x.PreviousQuotationId)
            .OnDelete(DeleteBehavior.Restrict);

        // Unique constraint for QuotationReference + Version
        builder.HasIndex(x => new { x.QuotationReference, x.Version })
            .IsUnique();

        builder.HasMany(x => x.LineItems)
            .WithOne(li => li.Quotation)
            .HasForeignKey(li => li.QuotationId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
