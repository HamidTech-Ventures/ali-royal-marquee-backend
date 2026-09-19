using AliRoyalMarquee.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AliRoyalMarquee.Infrastructure.Persistence.Configurations;

public class QuotationLineItemConfiguration : IEntityTypeConfiguration<QuotationLineItem>
{
    public void Configure(EntityTypeBuilder<QuotationLineItem> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Category)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Description)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(x => x.Quantity)
            .HasPrecision(18, 4); // allow fractional quantities

        builder.Property(x => x.Unit)
            .HasMaxLength(50);

        builder.Property(x => x.UnitPrice)
            .HasPrecision(18, 2);

        builder.Property(x => x.LineTotal)
            .HasPrecision(18, 2);

        builder.HasOne(x => x.Quotation)
            .WithMany(q => q.LineItems)
            .HasForeignKey(x => x.QuotationId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
