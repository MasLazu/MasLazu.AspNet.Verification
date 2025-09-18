using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MasLazu.AspNet.Verification.Domain.Entities;
using MasLazu.AspNet.Verification.Abstraction.Enums;

namespace MasLazu.AspNet.Verification.EfCore.Configurations;

public class VerificationConfiguration : IEntityTypeConfiguration<Domain.Entities.Verification>
{
    public void Configure(EntityTypeBuilder<Domain.Entities.Verification> builder)
    {
        builder.HasKey(v => v.Id);

        builder.Property(v => v.UserId)
            .IsRequired();

        builder.Property(v => v.Channel)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(v => v.Destination)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(v => v.VerificationCode)
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(v => v.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasDefaultValue(VerificationStatus.Pending);

        builder.Property(v => v.AttemptCount)
            .HasDefaultValue(0);

        builder.Property(v => v.ExpiresAt)
            .IsRequired();

        builder.Property(v => v.VerifiedAt)
            .IsRequired(false);

        builder.HasOne(v => v.VerificationPurpose)
            .WithMany(vp => vp.Verifications)
            .HasForeignKey(v => v.VerificationPurposeCode)
            .HasPrincipalKey(vp => vp.Code)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(v => new { v.UserId, v.VerificationPurposeCode, v.Status });

        builder.HasIndex(v => v.VerificationCode);

        builder.HasIndex(v => v.ExpiresAt);
    }
}
