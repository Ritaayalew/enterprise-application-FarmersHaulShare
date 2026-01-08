namespace HaulShareCreationAndScheduling.Domain.ValueObjects;

public record CapacityPlan(decimal TotalWeightKg, decimal MaxWeightKg)
{
    public bool Fits() => TotalWeightKg <= MaxWeightKg;
}
