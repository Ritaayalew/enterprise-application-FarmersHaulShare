using FarmersHaulShare.SharedKernel.Domain;
using PricingAndFairCostSplit.Domain.ValueObjects;

namespace PricingAndFairCostSplit.Domain.Events;

public record FairCostSplitDetermined(Guid HaulShareId, List<CostShare> FarmerShares, Money TotalCost) : IDomainEvent
{
    public DateTime OccurredOn => DateTime.UtcNow;
}