namespace PricingAndFairCostSplit.Domain.ValueObjects;

public record Money(decimal Amount, string Currency = "ETB")
{
    public Money() : this(0) { } // For EF

    public Money Add(Money other) => new(Amount + other.Amount, Currency);
    public Money Subtract(Money other) => new(Amount - other.Amount, Currency);
    public Money Multiply(decimal factor) => new(Amount * factor, Currency);

    public override string ToString() => $"{Amount:F2} {Currency}";
}