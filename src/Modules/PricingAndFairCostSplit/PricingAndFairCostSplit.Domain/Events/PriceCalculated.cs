using FarmersHaulShare.SharedKernel.Domain;
using PricingAndFairCostSplit.Domain.ValueObjects;

namespace PricingAndFairCostSplit.Domain.Events;

public record PriceCalculated(Guid HaulShareId, Money TotalRevenue, PricePerKg PricePerKg) : IDomainEvent
{
    public DateTime OccurredOn => DateTime.UtcNow;
}