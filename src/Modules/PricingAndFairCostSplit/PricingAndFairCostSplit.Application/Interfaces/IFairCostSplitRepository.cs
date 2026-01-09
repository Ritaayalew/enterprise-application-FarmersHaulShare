namespace PricingAndFairCostSplit.Application.Interfaces;

using PricingAndFairCostSplit.Domain.Aggregates;

public interface IFairCostSplitRepository
{
    Task AddAsync(FairCostSplit fairCostSplit);
    Task<FairCostSplit?> GetByHaulShareIdAsync(Guid haulShareId);
}
