 using HaulShareCreationAndScheduling.Domain.Aggregates;
using HaulShareCreationAndScheduling.Domain.Entities;
using HaulShareCreationAndScheduling.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace HaulShareCreationAndScheduling.Infrastructure.Persistence;

public class HaulShareDbContext : DbContext
{
    public DbSet<HaulShare> HaulShares => Set<HaulShare>();
    public DbSet<PickupStop> PickupStops => Set<PickupStop>();

    public HaulShareDbContext(DbContextOptions<HaulShareDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // ======================
        // HaulShare aggregate
        // ======================
        modelBuilder.Entity<HaulShare>(builder =>
        {
            builder.HasKey(h => h.Id);

            // Owned Value Object: CapacityPlan
            builder.OwnsOne(h => h.CapacityPlan, cp =>
            {
                cp.Property(p => p.TotalWeightKg);
                cp.Property(p => p.MaxWeightKg);
            });

            // Owned Value Object: DeliveryWindow
            builder.OwnsOne(h => h.DeliveryWindow, dw =>
            {
                dw.Property(d => d.Earliest);
                dw.Property(d => d.Latest);
            });

            // One-to-many PickupStops
            builder.HasMany(h => h.PickupStops)
                   .WithOne()
                   .HasForeignKey("HaulShareId");
        });

        // ======================
        // PickupStop entity
        // ======================
        modelBuilder.Entity<PickupStop>()
            .HasKey(p => p.Id);
    }
}
