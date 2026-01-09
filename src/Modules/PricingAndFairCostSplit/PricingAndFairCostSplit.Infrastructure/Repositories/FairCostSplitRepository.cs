using Microsoft.EntityFrameworkCore;
using PricingAndFairCostSplit.Domain.Aggregates;
using PricingAndFairCostSplit.Domain.Repositories;

namespace PricingAndFairCostSplit.Infrastructure.Repositories
{
    public class FairCostSplitRepository : IFairCostSplitRepository
    {
        private readonly PricingDbContext _context;

        public FairCostSplitRepository(PricingDbContext context)
        {
            _context = context;
        }

        public async Task<FairCostSplit> GetByHaulShareIdAsync(Guid haulShareId)
            => await _context.FairCostSplits
                .Include(f => f.FarmerShares)
                .FirstOrDefaultAsync(f => f.HaulShareId == haulShareId);

        public async Task AddAsync(FairCostSplit fairCostSplit)
        {
            _context.FairCostSplits.Add(fairCostSplit);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(FairCostSplit fairCostSplit)
        {
            _context.FairCostSplits.Update(fairCostSplit);
            await _context.SaveChangesAsync();
        }
    }
}
