using MessagingAndNotifications.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Domain;

namespace MessagingAndNotifications.Infrastructure.Data;

/// <summary>
/// EF Core DbContext for MessagingAndNotifications module
/// </summary>
public sealed class MessagingDbContext : DbContext
{
    public DbSet<Notification> Notifications { get; set; } = null!;

    public MessagingDbContext(DbContextOptions<MessagingDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(MessagingDbContext).Assembly);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker.Entries()
            .Where(e => e.Entity is Entity<Guid> &&
                        (e.State == EntityState.Added || e.State == EntityState.Modified));

        foreach (var entry in entries)
        {
            if (entry.Entity is Notification notification)
            {
                if (entry.State == EntityState.Added)
                {
                    var createdAtProperty = entry.Property(nameof(Notification.CreatedAtUtc));
                    if (createdAtProperty.CurrentValue is DateTime dt && dt == default(DateTime) || createdAtProperty.CurrentValue == null)
                        createdAtProperty.CurrentValue = DateTime.UtcNow;
                }
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}
