using Microsoft.EntityFrameworkCore;
using PricingAndFairCostSplit.Domain.Aggregates;
using PricingAndFairCostSplit.Domain.ValueObjects;

namespace PricingAndFairCostSplit.Infrastructure
{
    public class PricingDbContext : DbContext
    {
        public PricingDbContext(DbContextOptions<PricingDbContext> options)
            : base(options)
        {
        }

        public DbSet<FairCostSplit> FairCostSplits => Set<FairCostSplit>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FairCostSplit>(b =>
            {
                b.ToTable("FairCostSplits");

                b.HasKey(f => f.HaulShareId);

                b.Property(f => f.TotalRevenue)
                    .HasConversion(
                        v => v.Amount,
                        v => new Money(v)
                    )
                    .IsRequired();

                b.Property(f => f.TotalTransportCost)
                    .HasConversion(
                        v => v.Amount,
                        v => new Money(v)
                    )
                    .IsRequired();

                b.OwnsMany(f => f.FarmerShares, fs =>
                {
                    fs.ToTable("FarmerCostShares");

                    fs.WithOwner()
                      .HasForeignKey("FairCostSplitId");

                    fs.HasKey("Id"); // shadow key for owned entity
                    fs.Property<Guid>("Id");

                    fs.Property(f => f.FarmerId)
                      .IsRequired();

                    fs.Property(f => f.ShareAmount)
                      .HasConversion(
                          v => v.Amount,
                          v => new Money(v)
                      )
                      .IsRequired();

                    fs.Property(f => f.Percentage)
                      .IsRequired();
                });
            });
        }
    }
}
