using Microsoft.EntityFrameworkCore;
using TransportMarketplaceAndDispatch.Domain.Aggregates;
using TransportMarketplaceAndDispatch.Domain.Entities;
using FarmersHaulShare.SharedKernel;
using SharedKernel.Domain;

namespace TransportMarketplaceAndDispatch.Infrastructure.Persistence;

public sealed class TransportDbContext : DbContext
{
    public DbSet<Driver> Drivers => Set<Driver>();
    public DbSet<DispatchJob> DispatchJobs => Set<DispatchJob>();
    public DbSet<Vehicle> Vehicles => Set<Vehicle>();
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    public TransportDbContext(DbContextOptions<TransportDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Driver Aggregate
        modelBuilder.Entity<Driver>(builder =>
        {
            builder.HasKey(d => d.Id);

            builder.OwnsOne(d => d.CurrentLocation, loc =>
            {
                loc.Property(l => l.Latitude);
                loc.Property(l => l.Longitude);
            });

            builder.OwnsOne(d => d.AvailabilityWindow, aw =>
            {
                aw.Property(a => a.StartTime);
                aw.Property(a => a.EndTime);
                aw.Property(a => a.AvailableDays)
                  .HasConversion(
                      v => string.Join(",", v.Select(d => d.ToString())),
                      v => v.Split(",", StringSplitOptions.RemoveEmptyEntries)
                            .Select(d => Enum.Parse<DayOfWeek>(d))
                            .ToArray()
                  );
            });

            builder.HasMany<Vehicle>()
                   .WithOne()
                   .HasForeignKey("DriverId")
                   .OnDelete(DeleteBehavior.Cascade);

            builder.Ignore(d => d.DomainEvents);
        });

        // DispatchJob Aggregate
        modelBuilder.Entity<DispatchJob>(builder =>
        {
            builder.HasKey(dj => dj.Id);

            builder.OwnsOne(dj => dj.Route, route =>
            {
                route.OwnsOne(r => r.Origin, origin =>
                {
                    origin.Property(o => o.Latitude);
                    origin.Property(o => o.Longitude);
                });

                route.OwnsOne(r => r.Destination, dest =>
                {
                    dest.Property(d => d.Latitude);
                    dest.Property(d => d.Longitude);
                });

                route.OwnsMany(r => r.Waypoints, waypoint =>
                {
                    waypoint.Property(w => w.Latitude);
                    waypoint.Property(w => w.Longitude);
                });

                route.Property(r => r.EstimatedDistanceKm);
                route.Property(r => r.EstimatedDuration)
                      .HasConversion(
                          v => v.Ticks,
                          v => TimeSpan.FromTicks(v)
                      );
            });

            builder.OwnsOne(dj => dj.CurrentLocation, loc =>
            {
                loc.Property(l => l.Latitude);
                loc.Property(l => l.Longitude);
            });

            builder.Property(dj => dj.Status)
                   .HasConversion<string>();

            builder.Ignore(dj => dj.DomainEvents);
        });

        // Vehicle Entity
        modelBuilder.Entity<Vehicle>(builder =>
        {
            builder.HasKey(v => v.Id);

            builder.OwnsOne(v => v.Type, type =>
            {
                type.Property(t => t.Name);
                type.Property(t => t.MaxWeightKg);
                type.Property(t => t.MaxVolumeCubicMeters);
            });

            builder.Property(v => v.DriverId);
        });

        // OutboxMessage
        modelBuilder.Entity<OutboxMessage>(builder =>
        {
            builder.HasKey(o => o.Id);
            builder.Property(o => o.EventType).IsRequired();
            builder.Property(o => o.Payload).IsRequired();
            builder.Property(o => o.Status)
                   .HasConversion<string>();
        });
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Collect domain events before saving
        var domainEvents = ChangeTracker.Entries<AggregateRoot<Guid>>()
            .SelectMany(e => e.Entity.DomainEvents)
            .ToList();

        // Save events to outbox
        foreach (var domainEvent in domainEvents)
        {
            var outboxMessage = new OutboxMessage(domainEvent);
            OutboxMessages.Add(outboxMessage);
        }

        // Save all changes (entities + outbox messages) in a single transaction
        // Note: Domain events will be cleared when aggregates are reloaded from database
        return await base.SaveChangesAsync(cancellationToken);
    }
}