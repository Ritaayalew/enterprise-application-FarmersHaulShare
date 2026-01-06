using Microsoft.EntityFrameworkCore;
using FarmersHaulShare.SharedKernel;
using FarmersHaulShare.BatchPosting.Domain.Aggregates; // For Batch aggregate
using System.Text.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FarmersHaulShare.BatchPosting.Infrastructure;

public class BatchPostingDbContext : DbContext
{
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();
    public DbSet<Batch> Batches => Set<Batch>();

    public BatchPostingDbContext(DbContextOptions<BatchPostingDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
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

        // You can add Batch configuration here later if needed

        base.OnModelCreating(modelBuilder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Collect domain events from tracked entities that implement IHaveDomainEvents
        var entitiesWithEvents = ChangeTracker
            .Entries()
            .Where(e => e.Entity is IHaveDomainEvents)
            .Select(e => e.Entity as IHaveDomainEvents)
            .Where(e => e?.DomainEvents?.Any() == true)
            .ToList();

        var domainEvents = entitiesWithEvents
            .SelectMany(e => e!.DomainEvents)
            .ToList();

        // Create OutboxMessage using the new constructor that stores assembly-qualified type name
        foreach (var domainEvent in domainEvents)
        {
            var outboxMessage = new OutboxMessage(domainEvent);
            OutboxMessages.Add(outboxMessage);
        }

        // Clear events so they are not raised again on subsequent saves
        foreach (var entity in entitiesWithEvents)
        {
            entity.ClearDomainEvents();
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}