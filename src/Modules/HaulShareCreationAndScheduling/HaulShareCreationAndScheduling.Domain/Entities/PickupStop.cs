namespace HaulShareCreationAndScheduling.Domain.Entities;

public class PickupStop
{
    public Guid FarmerId { get; private set; }
    public decimal WeightKg { get; private set; }
    public double Latitude { get; private set; }
    public double Longitude { get; private set; }

    private PickupStop() { }

    public PickupStop(Guid farmerId, double latitude, double longitude, decimal weightKg)
    {
        FarmerId = farmerId;
        Latitude = latitude;
        Longitude = longitude;
        WeightKg = weightKg;
    }
}
