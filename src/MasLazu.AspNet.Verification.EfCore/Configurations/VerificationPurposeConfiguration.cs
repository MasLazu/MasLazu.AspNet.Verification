using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MasLazu.AspNet.Verification.Domain.Entities;

namespace MasLazu.AspNet.Verification.EfCore.Configurations;

public class VerificationPurposeConfiguration : IEntityTypeConfiguration<VerificationPurpose>
{
    public void Configure(EntityTypeBuilder<VerificationPurpose> builder)
    {
        builder.HasKey(vp => vp.Id);

        builder.Property(vp => vp.Code)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(vp => vp.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(vp => vp.Description)
            .HasMaxLength(500);

        builder.Property(vp => vp.IsActive)
            .HasDefaultValue(true);

        builder.HasIndex(vp => vp.Code)
            .IsUnique();

        builder.HasIndex(vp => vp.IsActive);
    }
}
