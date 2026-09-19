using AliRoyalMarquee.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AliRoyalMarquee.Infrastructure.Persistence.Configurations;

public class RefreshSessionConfiguration : IEntityTypeConfiguration<RefreshSession>
{
    public void Configure(EntityTypeBuilder<RefreshSession> builder)
    {
        builder.HasKey(rs => rs.Id);
        
        // Lookup optimizations
        builder.HasIndex(rs => rs.TokenHash);
        builder.HasIndex(rs => rs.UserId);
        
        builder.HasOne(rs => rs.User)
            .WithMany(u => u.RefreshSessions)
            .HasForeignKey(rs => rs.UserId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasOne(rs => rs.ReplacedBySession)
            .WithMany()
            .HasForeignKey(rs => rs.ReplacedBySessionId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
