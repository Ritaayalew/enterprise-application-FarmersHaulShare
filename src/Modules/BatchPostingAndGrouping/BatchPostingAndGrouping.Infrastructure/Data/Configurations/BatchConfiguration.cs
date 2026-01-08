using BatchPostingAndGrouping.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BatchPostingAndGrouping.Infrastructure.Data.Configurations;

public sealed class BatchConfiguration : IEntityTypeConfiguration<Batch>
{
    public void Configure(EntityTypeBuilder<Batch> builder)
    {
        builder.ToTable("Batches");

        builder.HasKey(b => b.Id);

        builder.Property(b => b.Id)
            .ValueGeneratedNever();

        builder.Property(b => b.FarmerProfileId)
            .IsRequired();

        // Value object: ProduceType (owned entity)
        builder.OwnsOne(b => b.ProduceType, produceType =>
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

        // Value object: QualityGrade (owned entity)
        builder.OwnsOne(b => b.QualityGrade, qualityGrade =>
        {
            qualityGrade.Property(qg => qg!.Grade)
                .HasColumnName("QualityGrade")
                .IsRequired()
                .HasMaxLength(50);

            qualityGrade.Property(qg => qg!.Description)
                .HasColumnName("QualityGradeDescription")
                .HasMaxLength(500);
        });

        builder.Property(b => b.WeightInKg)
            .IsRequired()
            .HasPrecision(18, 2);

        // Value object: Location (owned entity)
        builder.OwnsOne(b => b.PickupLocation, location =>
        {
            location.Property(l => l!.Latitude)
                .HasColumnName("PickupLatitude")
                .IsRequired();

            location.Property(l => l!.Longitude)
                .HasColumnName("PickupLongitude")
                .IsRequired();

            location.Property(l => l!.Address)
                .HasColumnName("PickupAddress")
                .HasMaxLength(500);
        });

        builder.Property(b => b.ReadyDateUtc)
            .IsRequired();

        builder.Property(b => b.PickupWindowEndUtc);

        builder.Property(b => b.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(b => b.CreatedAtUtc)
            .IsRequired();

        builder.Property(b => b.UpdatedAtUtc);

        builder.Property(b => b.CancelledAtUtc);

        builder.Property(b => b.CancellationReason)
            .HasMaxLength(500);

        // Navigation property
        builder.HasOne(b => b.FarmerProfile)
            .WithMany(f => f.Batches)
            .HasForeignKey(b => b.FarmerProfileId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(b => b.FarmerProfileId);
        builder.HasIndex(b => new { b.Status, b.ReadyDateUtc });
    }
}
