using SharedKernel.Domain;

namespace BatchPostingAndGrouping.Domain.DomainEvents;

/// <summary>
/// Domain event raised when a farmer posts a new batch
/// </summary>
public sealed record BatchPosted : IDomainEvent
{
    public Guid BatchId { get; init; }
    public Guid FarmerProfileId { get; init; }
    public string ProduceTypeName { get; init; } = string.Empty;
    public double WeightInKg { get; init; }
    public double Latitude { get; init; }
    public double Longitude { get; init; }
    public DateTime ReadyDateUtc { get; init; }
    public DateTime OccurredOnUtc { get; init; }

    private BatchPosted() { } // For deserialization

    public BatchPosted(
        Guid batchId,
        Guid farmerProfileId,
        string produceTypeName,
        double weightInKg,
        double latitude,
        double longitude,
        DateTime readyDateUtc)
    {
        BatchId = batchId;
        FarmerProfileId = farmerProfileId;
        ProduceTypeName = produceTypeName;
        WeightInKg = weightInKg;
        Latitude = latitude;
        Longitude = longitude;
        ReadyDateUtc = readyDateUtc;
        OccurredOnUtc = DateTime.UtcNow;
    }
}
