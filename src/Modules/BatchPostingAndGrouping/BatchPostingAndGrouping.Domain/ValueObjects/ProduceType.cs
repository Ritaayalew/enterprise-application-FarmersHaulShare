namespace BatchPostingAndGrouping.Domain.ValueObjects;

/// <summary>
/// Value object representing the type of produce
/// </summary>
public sealed class ProduceType : SharedKernel.Domain.ValueObject
{
    public string Name { get; private init; } = string.Empty;
    public string? Category { get; private init; } // e.g., "Vegetable", "Fruit", "Grain"
    public string? Unit { get; private init; } // e.g., "kg", "crate", "bag"

    private ProduceType() { } // EF Core

    public ProduceType(string name, string? category = null, string? unit = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new SharedKernel.Domain.DomainException("Produce type name cannot be empty.");

        Name = name.Trim();
        Category = category?.Trim();
        Unit = unit?.Trim();
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Name.ToLowerInvariant();
        yield return Category?.ToLowerInvariant();
    }

    public override string ToString() => Name;
}
