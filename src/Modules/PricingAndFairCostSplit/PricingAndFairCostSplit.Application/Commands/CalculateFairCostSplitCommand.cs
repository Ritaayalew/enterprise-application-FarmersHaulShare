using PricingAndFairCostSplit.Domain.Aggregates;
using PricingAndFairCostSplit.Domain.ValueObjects;

namespace PricingAndFairCostSplit.Application.Commands;

public record CalculateFairCostSplitCommand(
    Guid HaulShareId,
    PricePerKg PricePerKg,
    decimal TotalKg,
    decimal TotalTransportCost
);
