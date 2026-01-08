namespace BatchPostingAndGrouping.Application.DTOs;

public sealed record PostBatchDto
{
    public required string ProduceTypeName { get; init; }
    public string? ProduceTypeCategory { get; init; }
    public string? ProduceTypeUnit { get; init; }
    public required string QualityGrade { get; init; }
    public string? QualityGradeDescription { get; init; }
    public required double WeightInKg { get; init; }
    public required double Latitude { get; init; }
    public required double Longitude { get; init; }
    public string? Address { get; init; }
    public required DateTime ReadyDateUtc { get; init; }
    public DateTime? PickupWindowEndUtc { get; init; }
}
