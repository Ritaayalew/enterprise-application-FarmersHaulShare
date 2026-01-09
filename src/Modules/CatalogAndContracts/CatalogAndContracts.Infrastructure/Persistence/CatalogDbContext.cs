using Microsoft.EntityFrameworkCore;
using CatalogAndContracts.Domain.Aggregates;
using CatalogAndContracts.Domain.Entities;

namespace CatalogAndContracts.Infrastructure.Persistence
{
    public class CatalogDbContext : DbContext
    {
        public CatalogDbContext(DbContextOptions<CatalogDbContext> options)
            : base(options) { }

        public DbSet<Contract> Contracts => Set<Contract>();
        public DbSet<Product> Products => Set<Product>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Contract mapping
            modelBuilder.Entity<Contract>(builder =>
            {
                builder.ToTable("Contracts");
                builder.HasKey(c => c.Id);

                builder.Property(c => c.Price)
                       .IsRequired()
                       .HasColumnType("decimal(18,2)");

                builder.Property(c => c.BuyerId)
                       .IsRequired()
                       .HasMaxLength(100);

                builder.Property(c => c.FarmerId)
                       .IsRequired()
                       .HasMaxLength(100);

                builder.HasOne(c => c.Product)
                       .WithMany()
                       .HasForeignKey(c => c.ProductId)
                       .OnDelete(DeleteBehavior.Restrict);

                builder.OwnsOne(c => c.Terms, terms =>
                {
                    terms.Property(t => t.DeliveryDays)
                         .HasColumnName("DeliveryDays")
                         .IsRequired();

                    terms.Property(t => t.QualityStandard)
                         .HasColumnName("QualityStandard")
                         .HasMaxLength(200)
                         .IsRequired();
                });
            });

            // Product mapping
            modelBuilder.Entity<Product>(builder =>
            {
                builder.ToTable("Products");
                builder.HasKey(p => p.Id);

                builder.Property(p => p.Name)
                       .IsRequired()
                       .HasMaxLength(200);

                builder.Property(p => p.BasePrice)
                       .IsRequired()
                       .HasColumnType("decimal(18,2)");
            });
        }
    }
}