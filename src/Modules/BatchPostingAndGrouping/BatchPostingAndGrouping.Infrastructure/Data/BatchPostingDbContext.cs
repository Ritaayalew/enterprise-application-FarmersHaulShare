using BatchPostingAndGrouping.Domain.Aggregates;
using BatchPostingAndGrouping.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using SharedKernel.Domain;
using FarmersHaulShare.SharedKernel;

namespace BatchPostingAndGrouping.Infrastructure.Data;

public sealed class BatchPostingDbContext : DbContext
{
    public DbSet<FarmerProfile> FarmerProfiles { get; set; } = null!;
    public DbSet<Batch> Batches { get; set; } = null!;
    public DbSet<GroupCandidate> GroupCandidates { get; set; } = null!;

    // ADD THIS LINE – the missing DbSet for Outbox
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    public BatchPostingDbContext(DbContextOptions<BatchPostingDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply configurations (if you have any)
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(BatchPostingDbContext).Assembly);

        // Optional: Outbox table config (if not already in SharedKernel)
        modelBuilder.Entity<OutboxMessage>(entity =>
        {
            entity.ToTable("OutboxMessages");
            entity.HasKey(o => o.Id);
            entity.Property(o => o.Id).ValueGeneratedNever();
            entity.Property(o => o.EventType).HasMaxLength(500).IsRequired();
            entity.Property(o => o.Payload).HasColumnType("text").IsRequired();
            entity.Property(o => o.OccurredOn).IsRequired();
            entity.Property(o => o.ProcessedOn);
            entity.Property(o => o.Status).HasConversion<string>();
            entity.Property(o => o.RetryCount);
        });
    }

    // Your existing timestamp logic...
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // ... your timestamp code ...

        // ADD THIS: Capture domain events to Outbox (critical for pattern)
        var entitiesWithEvents = ChangeTracker
            .Entries<AggregateRoot<Guid>>()
            .Where(e => e.Entity.DomainEvents.Any())
            .Select(e => e.Entity)
            .ToList();

        var domainEvents = entitiesWithEvents
            .SelectMany(e => e.DomainEvents)
            .ToList();

        foreach (var entity in entitiesWithEvents)
        {
            entity.ClearDomainEvents();
        }

        foreach (var domainEvent in domainEvents)
        {
            var outboxMessage = new OutboxMessage(domainEvent);
            OutboxMessages.Add(outboxMessage);
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}