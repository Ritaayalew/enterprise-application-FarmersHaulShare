 namespace HaulShareCreationAndScheduling.Domain.Entities;

public class PickupStop
{
    public Guid Id { get; private set; }   // ✅ Primary Key (REQUIRED by EF)

    public Guid FarmerId { get; private set; }
    public decimal WeightKg { get; private set; }
    public double Latitude { get; private set; }
    public double Longitude { get; private set; }

    private PickupStop() { } // ✅ EF Core

    public PickupStop(
        Guid farmerId,
        double latitude,
        double longitude,
        decimal weightKg)
    {
        Id = Guid.NewGuid();   // ✅ Ensure PK exists
        FarmerId = farmerId;
        Latitude = latitude;
        Longitude = longitude;
        WeightKg = weightKg;
    }
}
