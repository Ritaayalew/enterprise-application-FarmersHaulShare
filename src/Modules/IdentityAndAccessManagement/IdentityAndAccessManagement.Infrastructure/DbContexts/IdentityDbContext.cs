using IdentityAndAccessManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using FarmersHaulShare.SharedKernel;  // For OutboxMessage
using SharedKernel.Domain;  // For AggregateRoot<Guid>

namespace IdentityAndAccessManagement.Infrastructure.DbContexts;

public class IdentityDbContext : DbContext
{
    public DbSet<User> Users => Set<User>();
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    public IdentityDbContext(DbContextOptions<IdentityDbContext> options)
        : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseNpgsql(
                "your-connection-string",
                x => x.MigrationsHistoryTable("__EFMigrationsHistory", "iam")
            );
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // SCHEMA ISOLATION – EACH MODULE OWNS ITS OWN SCHEMA
        modelBuilder.HasDefaultSchema("iam");

        // User configuration with owned value objects
        modelBuilder.Entity<User>(b =>
        {
            b.HasKey(u => u.Id);

            b.OwnsOne(u => u.Email, e =>
            {
                e.Property(x => x.Value).HasColumnName("Email").IsRequired();
            });

            b.OwnsOne(u => u.Name, n =>
            {
                n.Property(x => x.FirstName).HasColumnName("FirstName").IsRequired();
                n.Property(x => x.LastName).HasColumnName("LastName").IsRequired();
            });

            b.OwnsOne(u => u.PhoneNumber, p =>
            {
                p.Property(x => x.Value).HasColumnName("PhoneNumber");
            });

            b.OwnsOne(u => u.CooperativeId, c =>
            {
                c.Property(x => x.Value).HasColumnName("CooperativeId");
            });

            b.Property(u => u.KeycloakSubjectId).IsRequired();
            b.Property(u => u.LastLoginAt);
            b.Property(u => u.HasCompletedOnboarding);

            b.Ignore(u => u.Roles);
        });

        // Outbox configuration
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

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
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
