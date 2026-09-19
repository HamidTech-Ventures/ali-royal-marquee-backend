using AliRoyalMarquee.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AliRoyalMarquee.Infrastructure.Persistence.Configurations;

public class ExpenseConfiguration : IEntityTypeConfiguration<Expense>
{
    public void Configure(EntityTypeBuilder<Expense> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Category).IsRequired().HasMaxLength(100);
        builder.Property(e => e.Description).IsRequired().HasMaxLength(500);
        builder.Property(e => e.Amount).HasColumnType("numeric(18,2)");
        
        builder.ToTable(t => t.HasCheckConstraint("CHK_Expense_Amount", "\"Amount\" > 0"));
        
        builder.HasIndex(e => e.ExpenseDate);
    }
}
