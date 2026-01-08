using SharedKernel.Domain;
using TransportMarketplaceAndDispatch.Domain.ValueObjects;
using System.Diagnostics.CodeAnalysis;

namespace TransportMarketplaceAndDispatch.Domain.Entities;

public sealed class Vehicle : Entity<Guid>
{
    public Guid DriverId { get; private set; }
    public string PlateNumber { get; private set; } = string.Empty;
    public VehicleType Type { get; private set; } = null!;
    public bool IsVerified { get; private set; }
    public DateTime? VerifiedAt { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }

    private Vehicle() { } // EF Core

    [SetsRequiredMembers]
    public Vehicle(Guid id, Guid driverId, string plateNumber, VehicleType type)
        : base(id)
    {
        if (string.IsNullOrWhiteSpace(plateNumber))
            throw new ArgumentException("Plate number cannot be empty", nameof(plateNumber));

        Id = id; // Explicitly set for required property
        DriverId = driverId;
        PlateNumber = plateNumber;
        Type = type ?? throw new ArgumentNullException(nameof(type));
        IsVerified = false;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public void Verify()
    {
        if (IsVerified)
            throw new InvalidOperationException("Vehicle is already verified");

        IsVerified = true;
        VerifiedAt = DateTime.UtcNow;
    }
}