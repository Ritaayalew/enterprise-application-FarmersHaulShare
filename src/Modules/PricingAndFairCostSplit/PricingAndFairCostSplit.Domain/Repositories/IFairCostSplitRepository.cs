using PricingAndFairCostSplit.Domain.Aggregates;

namespace PricingAndFairCostSplit.Domain.Repositories
{
    public interface IFairCostSplitRepository
    {
        Task<FairCostSplit> GetByHaulShareIdAsync(Guid haulShareId);
        Task AddAsync(FairCostSplit fairCostSplit);
        Task UpdateAsync(FairCostSplit fairCostSplit);
    }
}
