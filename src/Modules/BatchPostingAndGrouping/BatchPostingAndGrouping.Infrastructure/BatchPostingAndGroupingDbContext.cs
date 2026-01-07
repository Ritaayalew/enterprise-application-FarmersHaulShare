using Microsoft.EntityFrameworkCore;
using FarmersHaulShare.SharedKernel;
using FarmersHaulShare.SharedKernel.Domain; // For IDomainEvent
using System.Text.Json;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FarmersHaulShare.BatchPostingAndGrouping.Domain.Aggregates;


namespace BatchPostingAndGrouping.Infrastructure;

public class BatchPostingAndGroupingDbContext : DbContext
{
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    // Add other DbSets for your aggregates here later (e.g., Batch, GroupCandidate)

    public BatchPostingAndGroupingDbContext(DbContextOptions<BatchPostingAndGroupingDbContext> options)
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

        base.OnModelCreating(modelBuilder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entitiesWithEvents = ChangeTracker
            .Entries()
            .Where(e => e.Entity is IHaveDomainEvents)
            .Select(e => e.Entity as IHaveDomainEvents)
            .Where(e => e?.DomainEvents?.Any() == true)
            .ToList();

        var domainEvents = entitiesWithEvents
            .SelectMany(e => e!.DomainEvents)
            .ToList();

        foreach (var domainEvent in domainEvents)
        {
            var outboxMessage = new OutboxMessage(domainEvent);
            OutboxMessages.Add(outboxMessage);
        }

        foreach (var entity in entitiesWithEvents)
        {
            entity.ClearDomainEvents();
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
    public DbSet<Batch> Batches => Set<Batch>();
}