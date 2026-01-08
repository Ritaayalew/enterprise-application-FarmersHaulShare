namespace BatchPostingAndGrouping.Application.DTOs;

public sealed record GroupCandidateDto
{
    public Guid Id { get; init; }
    public List<Guid> BatchIds { get; init; } = new();
    public string ProduceTypeName { get; init; } = string.Empty;
    public double TotalWeightInKg { get; init; }
    public DateTime EarliestReadyDateUtc { get; init; }
    public DateTime LatestReadyDateUtc { get; init; }
    public DateTime? PickupWindowEndUtc { get; init; }
    public string Status { get; init; } = string.Empty;
    public DateTime? LockWindowStartUtc { get; init; }
    public DateTime? LockWindowEndUtc { get; init; }
    public DateTime CreatedAtUtc { get; init; }
    public int BatchCount { get; init; }
}
