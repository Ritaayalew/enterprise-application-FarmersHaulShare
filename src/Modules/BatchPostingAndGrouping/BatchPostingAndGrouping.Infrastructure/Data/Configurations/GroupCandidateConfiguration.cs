using BatchPostingAndGrouping.Domain.Aggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BatchPostingAndGrouping.Infrastructure.Data.Configurations;

public sealed class GroupCandidateConfiguration : IEntityTypeConfiguration<GroupCandidate>
{
    public void Configure(EntityTypeBuilder<GroupCandidate> builder)
    {
        builder.ToTable("GroupCandidates");

        builder.HasKey(g => g.Id);

        builder.Property(g => g.Id)
            .ValueGeneratedNever();

        // Value object: ProduceType (owned entity)
        builder.OwnsOne(g => g.ProduceType, produceType =>
        {
            produceType.Property(pt => pt!.Name)
                .HasColumnName("ProduceTypeName")
                .IsRequired()
                .HasMaxLength(100);

            produceType.Property(pt => pt!.Category)
                .HasColumnName("ProduceTypeCategory")
                .HasMaxLength(50);

            produceType.Property(pt => pt!.Unit)
                .HasColumnName("ProduceTypeUnit")
                .HasMaxLength(20);
        });

        // Collection of batch IDs - stored as a private field
        // EF Core can access private fields using UsePropertyAccessMode.Field
        builder.Property<List<Guid>>("_batchIds")
            .HasColumnName("BatchIds")
            .HasConversion(
                v => v == null || v.Count == 0 
                    ? string.Empty 
                    : string.Join(',', v.Select(g => g.ToString())),
                v => string.IsNullOrWhiteSpace(v) 
                    ? new List<Guid>() 
                    : v.Split(',', StringSplitOptions.RemoveEmptyEntries)
                        .Where(s => !string.IsNullOrWhiteSpace(s))
                        .Select(Guid.Parse)
                        .ToList())
            .IsRequired()
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        // Ignore the public read-only property, EF Core will use the private field
        builder.Ignore(g => g.BatchIds);

        builder.Property(g => g.TotalWeightInKg)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(g => g.EarliestReadyDateUtc)
            .IsRequired();

        builder.Property(g => g.LatestReadyDateUtc)
            .IsRequired();

        builder.Property(g => g.PickupWindowEndUtc);

        builder.Property(g => g.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(g => g.LockWindowStartUtc);

        builder.Property(g => g.LockWindowEndUtc);

        builder.Property(g => g.CreatedAtUtc)
            .IsRequired();

        builder.Property(g => g.UpdatedAtUtc);

        builder.Property(g => g.LockedAtUtc);

        // Indexes
        builder.HasIndex(g => new { g.Status, g.ProduceType!.Name });
    }
}
