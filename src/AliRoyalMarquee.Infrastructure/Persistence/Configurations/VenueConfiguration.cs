using AliRoyalMarquee.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AliRoyalMarquee.Infrastructure.Persistence.Configurations;

public class VenueConfiguration : IEntityTypeConfiguration<Venue>
{
    public void Configure(EntityTypeBuilder<Venue> builder)
    {
        builder.HasKey(v => v.Id);
        
        builder.Property(v => v.Name).IsRequired().HasMaxLength(100);
        builder.HasIndex(v => v.Name).IsUnique();
        
        builder.Property(v => v.Description).HasMaxLength(500);
    }
}
