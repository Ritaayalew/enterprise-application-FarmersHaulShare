namespace PricingAndFairCostSplit.Domain.ValueObjects;

public record PricePerKg
{
    public Money AmountPerKg { get; init; }

    public PricePerKg(decimal amount)
    {
        AmountPerKg = new Money(amount);
    }
}
