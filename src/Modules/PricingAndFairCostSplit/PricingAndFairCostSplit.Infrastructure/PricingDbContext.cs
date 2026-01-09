using Microsoft.EntityFrameworkCore;
using PricingAndFairCostSplit.Domain.Aggregates;
using PricingAndFairCostSplit.Domain.ValueObjects;

namespace PricingAndFairCostSplit.Infrastructure
{
    public class PricingDbContext : DbContext
    {
        public PricingDbContext(DbContextOptions<PricingDbContext> options) : base(options) { }

        public DbSet<FairCostSplit> FairCostSplits { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FairCostSplit>(b =>
            {
                b.HasKey(f => f.HaulShareId);

                b.OwnsMany(f => f.FarmerShares, fs =>
                {
                    fs.WithOwner().HasForeignKey("FairCostSplitId");
                    fs.Property(f => f.FarmerId);
                    fs.Property(f => f.ShareAmount).HasConversion(
                        v => v.Amount,
                        v => new Money(v)
                    );
                    fs.Property(f => f.Percentage);
                });

                b.Property(f => f.TotalRevenue).HasConversion(
                    v => v.Amount,
                    v => new Money(v)
                );
                b.Property(f => f.TotalTransportCost).HasConversion(
                    v => v.Amount,
                    v => new Money(v)
                );
            });
        }
    }
}
