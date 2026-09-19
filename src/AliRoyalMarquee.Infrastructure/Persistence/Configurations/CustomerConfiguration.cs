using AliRoyalMarquee.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AliRoyalMarquee.Infrastructure.Persistence.Configurations;

public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.HasKey(c => c.Id);
        
        builder.Property(c => c.Name).IsRequired().HasMaxLength(100);
        builder.Property(c => c.Phone).IsRequired().HasMaxLength(20);
        builder.Property(c => c.Email).HasMaxLength(150);

        builder.HasIndex(c => c.Phone);
        builder.HasIndex(c => c.Email);
    }
}
