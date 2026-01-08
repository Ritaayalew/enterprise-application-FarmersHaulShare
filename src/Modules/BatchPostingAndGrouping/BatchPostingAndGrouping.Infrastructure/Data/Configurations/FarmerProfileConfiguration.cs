using BatchPostingAndGrouping.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BatchPostingAndGrouping.Infrastructure.Data.Configurations;

public sealed class FarmerProfileConfiguration : IEntityTypeConfiguration<FarmerProfile>
{
    public void Configure(EntityTypeBuilder<FarmerProfile> builder)
    {
        builder.ToTable("FarmerProfiles");

        builder.HasKey(f => f.Id);

        builder.Property(f => f.Id)
            .ValueGeneratedNever();

        builder.Property(f => f.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(f => f.PhoneNumber)
            .HasMaxLength(50);

        // Value object: Location (owned entity)
        builder.OwnsOne(f => f.DefaultPickupLocation, location =>
        {
            location.Property(l => l!.Latitude)
                .HasColumnName("DefaultLatitude")
                .IsRequired(false);

            location.Property(l => l!.Longitude)
                .HasColumnName("DefaultLongitude")
                .IsRequired(false);

            location.Property(l => l!.Address)
                .HasColumnName("DefaultAddress")
                .HasMaxLength(500);
        });

        builder.Property(f => f.CreatedAtUtc)
            .IsRequired();

        builder.Property(f => f.UpdatedAtUtc);

        // Navigation property
        builder.HasMany(f => f.Batches)
            .WithOne(b => b.FarmerProfile)
            .HasForeignKey(b => b.FarmerProfileId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(f => f.Batches)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
