namespace HaulShareCreationAndScheduling.Application.DTOs;

public class LockedBatchDto
{
    public Guid FarmerId { get; set; }
    public decimal WeightKg { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}
