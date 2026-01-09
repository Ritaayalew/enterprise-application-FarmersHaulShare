namespace PricingAndFairCostSplit.Domain.ValueObjects;

public record CostShare(Guid FarmerId, Money ShareAmount, decimal Percentage);