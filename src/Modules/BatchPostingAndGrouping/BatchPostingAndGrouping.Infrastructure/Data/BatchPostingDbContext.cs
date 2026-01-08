using BatchPostingAndGrouping.Domain.Aggregates;
using BatchPostingAndGrouping.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using SharedKernel.Domain;

namespace BatchPostingAndGrouping.Infrastructure.Data;

public sealed class BatchPostingDbContext : DbContext
{
    public DbSet<FarmerProfile> FarmerProfiles { get; set; } = null!;
    public DbSet<Batch> Batches { get; set; } = null!;
    public DbSet<GroupCandidate> GroupCandidates { get; set; } = null!;

    public BatchPostingDbContext(DbContextOptions<BatchPostingDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all configurations from the same assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(BatchPostingDbContext).Assembly);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Update timestamps
        var entries = ChangeTracker.Entries()
            .Where(e => e.Entity is Entity<Guid> && 
                       (e.State == EntityState.Added || e.State == EntityState.Modified));

        foreach (var entry in entries)
        {
            if (entry.Entity is FarmerProfile)
            {
                if (entry.State == EntityState.Added)
                {
                    // CreatedAtUtc is set in constructor, but ensure it's set
                    var createdAtProperty = entry.Property(nameof(FarmerProfile.CreatedAtUtc));
                    if (createdAtProperty.CurrentValue is null || createdAtProperty.CurrentValue.Equals(default(DateTime)))
                        createdAtProperty.CurrentValue = DateTime.UtcNow;
                }
                else if (entry.State == EntityState.Modified)
                {
                    entry.Property(nameof(FarmerProfile.UpdatedAtUtc)).CurrentValue = DateTime.UtcNow;
                }
            }
            else if (entry.Entity is Batch)
            {
                if (entry.State == EntityState.Added)
                {
                    var createdAtProperty = entry.Property(nameof(Batch.CreatedAtUtc));
                    if (createdAtProperty.CurrentValue is null || createdAtProperty.CurrentValue.Equals(default(DateTime)))
                        createdAtProperty.CurrentValue = DateTime.UtcNow;
                }
                else if (entry.State == EntityState.Modified)
                {
                    entry.Property(nameof(Batch.UpdatedAtUtc)).CurrentValue = DateTime.UtcNow;
                }
            }
            else if (entry.Entity is GroupCandidate)
            {
                if (entry.State == EntityState.Added)
                {
                    var createdAtProperty = entry.Property(nameof(GroupCandidate.CreatedAtUtc));
                    if (createdAtProperty.CurrentValue is null || createdAtProperty.CurrentValue.Equals(default(DateTime)))
                        createdAtProperty.CurrentValue = DateTime.UtcNow;
                }
                else if (entry.State == EntityState.Modified)
                {
                    entry.Property(nameof(GroupCandidate.UpdatedAtUtc)).CurrentValue = DateTime.UtcNow;
                }
            }
        }

        // Save changes
        var result = await base.SaveChangesAsync(cancellationToken);

        // Note: Domain events should be published by a domain event dispatcher
        // This is typically done in a decorator pattern or in the repository SaveChanges
        // For now, we'll handle it at the application service level

        return result;
    }
}
