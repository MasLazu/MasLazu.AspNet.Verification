using Microsoft.EntityFrameworkCore;
using MasLazu.AspNet.Framework.EfCore.Data;
using MasLazu.AspNet.Verification.Domain.Entities;
using System.Reflection;

namespace MasLazu.AspNet.Verification.EfCore.Data;

public class VerificationDbContext : BaseDbContext
{
    public VerificationDbContext(DbContextOptions<VerificationDbContext> options) : base(options)
    {
    }

    public DbSet<Domain.Entities.Verification> Verifications { get; set; }
    public DbSet<VerificationPurpose> VerificationPurposes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
