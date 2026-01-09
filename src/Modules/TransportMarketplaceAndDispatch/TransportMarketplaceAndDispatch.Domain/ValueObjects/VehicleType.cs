using SharedKernel.Domain;

namespace TransportMarketplaceAndDispatch.Domain.ValueObjects;

public sealed class VehicleType : ValueObject
{
    public string Name { get; } = string.Empty;
    public double MaxWeightKg { get; }
    public double MaxVolumeCubicMeters { get; }

    private VehicleType() { } // EF Core

    public VehicleType(string name, double maxWeightKg, double maxVolumeCubicMeters)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Vehicle type name cannot be empty", nameof(name));
        if (maxWeightKg <= 0)
            throw new ArgumentException("Max weight must be positive", nameof(maxWeightKg));
        if (maxVolumeCubicMeters <= 0)
            throw new ArgumentException("Max volume must be positive", nameof(maxVolumeCubicMeters));

        Name = name;
        MaxWeightKg = maxWeightKg;
        MaxVolumeCubicMeters = maxVolumeCubicMeters;
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Name;
        yield return MaxWeightKg;
        yield return MaxVolumeCubicMeters;
    }

    public static VehicleType Truck => new("Truck", 10000, 40);
    public static VehicleType Motorbike => new("Motorbike", 150, 0.5);
    public static VehicleType Van => new("Van", 2000, 8);
}