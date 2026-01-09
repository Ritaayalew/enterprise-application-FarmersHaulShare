using SharedKernel.Domain;

namespace BatchPostingAndGrouping.Domain.Entities;

/// <summary>
/// Entity representing a farmer's profile and calendar
/// </summary>
public sealed class FarmerProfile : Entity<Guid>
{
    public string Name { get; private set; } = string.Empty;
    public string? PhoneNumber { get; private set; }
    public ValueObjects.Location? DefaultPickupLocation { get; private set; }
    public DateTime CreatedAtUtc { get; private init; }
    public DateTime? UpdatedAtUtc { get; private set; }

    // Navigation property - batches posted by this farmer
    private readonly List<Batch> _batches = new();
    public IReadOnlyCollection<Batch> Batches => _batches.AsReadOnly();

    private FarmerProfile() { } // EF Core

    public FarmerProfile(
        Guid id,
        string name,
        string? phoneNumber = null,
        ValueObjects.Location? defaultPickupLocation = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new SharedKernel.Domain.DomainException("Farmer name cannot be empty.");

        Id = id;
        Name = name.Trim();
        PhoneNumber = phoneNumber?.Trim();
        DefaultPickupLocation = defaultPickupLocation;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public void UpdateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new SharedKernel.Domain.DomainException("Farmer name cannot be empty.");

        Name = name.Trim();
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void UpdatePhoneNumber(string? phoneNumber)
    {
        PhoneNumber = phoneNumber?.Trim();
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void UpdateDefaultPickupLocation(ValueObjects.Location? location)
    {
        DefaultPickupLocation = location;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    internal void AddBatch(Batch batch)
    {
        _batches.Add(batch);
    }
}
