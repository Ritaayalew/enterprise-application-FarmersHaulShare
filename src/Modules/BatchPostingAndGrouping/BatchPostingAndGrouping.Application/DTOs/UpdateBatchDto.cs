namespace BatchPostingAndGrouping.Application.DTOs;

public sealed record UpdateBatchDto
{
    public string? ProduceTypeName { get; init; }
    public string? ProduceTypeCategory { get; init; }
    public string? QualityGrade { get; init; }
    public double? WeightInKg { get; init; }
    public double? Latitude { get; init; }
    public double? Longitude { get; init; }
    public string? Address { get; init; }
    public DateTime? ReadyDateUtc { get; init; }
    public DateTime? PickupWindowEndUtc { get; init; }
}
