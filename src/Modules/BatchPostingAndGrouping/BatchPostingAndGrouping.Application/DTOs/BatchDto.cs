namespace BatchPostingAndGrouping.Application.DTOs;

public sealed record BatchDto
{
    public Guid Id { get; init; }
    public Guid FarmerProfileId { get; init; }
    public string ProduceTypeName { get; init; } = string.Empty;
    public string? ProduceTypeCategory { get; init; }
    public string QualityGrade { get; init; } = string.Empty;
    public double WeightInKg { get; init; }
    public double Latitude { get; init; }
    public double Longitude { get; init; }
    public string? Address { get; init; }
    public DateTime ReadyDateUtc { get; init; }
    public DateTime? PickupWindowEndUtc { get; init; }
    public string Status { get; init; } = string.Empty;
    public DateTime CreatedAtUtc { get; init; }
    public DateTime? UpdatedAtUtc { get; init; }
}
